using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;


/// <summary>
/// 修复UnityUGui中Canvas画布在Windows平台宽高比过长（横屏拉很长）导致UI的最右边一部分无法被点击的Bug
/// </summary>
public class GraphicRaycasterBugFixed : GraphicRaycaster {
    [NonSerialized] private List<Graphic> m_RaycastResults2 = new List<Graphic>();
    private Canvas m_Canvas2;

    private Canvas canvas2 {
        get {
            if (m_Canvas2 != null)
                return m_Canvas2;

            m_Canvas2 = GetComponent<Canvas>();
            return m_Canvas2;
        }
    }
    /// <summary>
    /// Perform the raycast against the list of graphics associated with the Canvas.
    /// </summary>
    /// <param name="eventData">Current event data</param>
    /// <param name="resultAppendList">List of hit objects to append new results to.</param>
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList) {
        if (canvas2 == null)
            return;

        var canvasGraphics = GraphicRegistry.GetRaycastableGraphicsForCanvas(canvas2);
        if (canvasGraphics == null || canvasGraphics.Count == 0)
            return;

        int displayIndex;
        var currentEventCamera = eventCamera; // Property can call Camera.main, so cache the reference

        if (canvas2.renderMode == RenderMode.ScreenSpaceOverlay || currentEventCamera == null)
            displayIndex = canvas2.targetDisplay;
        else
            displayIndex = currentEventCamera.targetDisplay;

        var eventPosition = Display.RelativeMouseAt(eventData.position);//MultipleDisplayUtilities.RelativeMouseAtScaled(eventData.position);
        if (eventPosition != Vector3.zero) {
            // We support multiple display and display identification based on event position.

            int eventDisplayIndex = (int)eventPosition.z;

            // Discard events that are not part of this display so the user does not interact with multiple displays at once.
            if (eventDisplayIndex != displayIndex)
                return;
        }
        else {
            // The multiple display system is not supported on all platforms, when it is not supported the returned position
            // will be all zeros so when the returned index is 0 we will default to the event data to be safe.
            eventPosition = eventData.position;

#if UNITY_EDITOR
            if (Display.activeEditorGameViewTarget != displayIndex)
                return;
            eventPosition.z = Display.activeEditorGameViewTarget;
#endif

            // We dont really know in which display the event occured. We will process the event assuming it occured in our display.
        }

        // Convert to view space
        Vector2 pos;
        if (currentEventCamera == null) {
            // Multiple display support only when not the main display. For display 0 the reported
            // resolution is always the desktops resolution since its part of the display API,
            // so we use the standard none multiple display method. (case 741751)
            float w = Screen.width;
            float h = Screen.height;
            if (displayIndex > 0 && displayIndex < Display.displays.Length) {
                w = Display.displays[displayIndex].systemWidth;
                h = Display.displays[displayIndex].systemHeight;
            }
            pos = new Vector2(eventPosition.x / w, eventPosition.y / h);
        }
        else
            pos = currentEventCamera.ScreenToViewportPoint(eventPosition);

        // If it's outside the camera's viewport, do nothing
        if (pos.x < 0f || pos.x > 1f || pos.y < 0f || pos.y > 1f)
            return;

        float hitDistance = float.MaxValue;

        Ray ray = new Ray();

        if (currentEventCamera != null)
            ray = currentEventCamera.ScreenPointToRay(eventPosition);

        if (canvas2.renderMode != RenderMode.ScreenSpaceOverlay && blockingObjects != BlockingObjects.None) {
            float distanceToClipPlane = 100.0f;

            if (currentEventCamera != null) {
                float projectionDirection = ray.direction.z;
                distanceToClipPlane = Mathf.Approximately(0.0f, projectionDirection)
                    ? Mathf.Infinity
                    : Mathf.Abs((currentEventCamera.farClipPlane - currentEventCamera.nearClipPlane) / projectionDirection);
            }
#if PACKAGE_PHYSICS
 
            if (blockingObjects == BlockingObjects.ThreeD || blockingObjects == BlockingObjects.All)
            {
                if (ReflectionMethodsCache2.Singleton.raycast3D != null)
                {
                    var hits = ReflectionMethodsCache2.Singleton.raycast3DAll(ray, distanceToClipPlane, (int)m_BlockingMask);
                    if (hits.Length > 0)
                        hitDistance = hits[0].distance;
                }
            }
#endif
#if PACKAGE_PHYSICS2D
 
 
            if (blockingObjects == BlockingObjects.TwoD || blockingObjects == BlockingObjects.All)
            {
                if (ReflectionMethodsCache2.Singleton.raycast2D != null)
                {
                    var hits = ReflectionMethodsCache2.Singleton.getRayIntersectionAll(ray, distanceToClipPlane, (int)m_BlockingMask);
                    if (hits.Length > 0)
                        hitDistance = hits[0].distance;
                }
            }
#endif
        }

        m_RaycastResults2.Clear();

        Raycast2(canvas2, currentEventCamera, eventPosition, canvasGraphics, m_RaycastResults2);

        int totalCount = m_RaycastResults2.Count;
        for (var index = 0; index < totalCount; index++) {
            var go = m_RaycastResults2[index].gameObject;
            bool appendGraphic = true;

            if (ignoreReversedGraphics) {
                if (currentEventCamera == null) {
                    // If we dont have a camera we know that we should always be facing forward
                    var dir = go.transform.rotation * Vector3.forward;
                    appendGraphic = Vector3.Dot(Vector3.forward, dir) > 0;
                }
                else {
                    // If we have a camera compare the direction against the cameras forward.
                    var cameraForward = currentEventCamera.transform.rotation * Vector3.forward * currentEventCamera.nearClipPlane;
                    appendGraphic = Vector3.Dot(go.transform.position - currentEventCamera.transform.position - cameraForward, go.transform.forward) >= 0;
                }
            }

            if (appendGraphic) {
                float distance = 0;
                Transform trans = go.transform;
                Vector3 transForward = trans.forward;

                if (currentEventCamera == null || canvas2.renderMode == RenderMode.ScreenSpaceOverlay)
                    distance = 0;
                else {
                    // http://geomalgorithms.com/a06-_intersect-2.html
                    distance = (Vector3.Dot(transForward, trans.position - ray.origin) / Vector3.Dot(transForward, ray.direction));

                    // Check to see if the go is behind the camera.
                    if (distance < 0)
                        continue;
                }

                if (distance >= hitDistance)
                    continue;

                var castResult = new RaycastResult {
                    gameObject = go,
                    module = this,
                    distance = distance,
                    screenPosition = eventPosition,
                    displayIndex = displayIndex,
                    index = resultAppendList.Count,
                    depth = m_RaycastResults2[index].depth,
                    sortingLayer = canvas2.sortingLayerID,
                    sortingOrder = canvas2.sortingOrder,
                    worldPosition = ray.origin + ray.direction * distance,
                    worldNormal = -transForward
                };
                resultAppendList.Add(castResult);
            }
        }
    }
    /// <summary>
    /// Perform a raycast into the screen and collect all graphics underneath it.
    /// </summary>
    [NonSerialized] static readonly List<Graphic> s_SortedGraphics2 = new List<Graphic>();
    private static void Raycast2(Canvas canvas, Camera eventCamera, Vector2 pointerPosition, IList<Graphic> foundGraphics, List<Graphic> results) {
        // Necessary for the event system
        int totalCount = foundGraphics.Count;
        for (int i = 0; i < totalCount; ++i) {
            Graphic graphic = foundGraphics[i];

            // -1 means it hasn't been processed by the canvas, which means it isn't actually drawn
            if (!graphic.raycastTarget || graphic.canvasRenderer.cull || graphic.depth == -1)
                continue;

            if (!RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera, graphic.raycastPadding))
                continue;

            if (eventCamera != null && eventCamera.WorldToScreenPoint(graphic.rectTransform.position).z > eventCamera.farClipPlane)
                continue;

            if (graphic.Raycast(pointerPosition, eventCamera)) {
                s_SortedGraphics2.Add(graphic);
            }
        }

        s_SortedGraphics2.Sort((g1, g2) => g2.depth.CompareTo(g1.depth));
        totalCount = s_SortedGraphics2.Count;
        for (int i = 0; i < totalCount; ++i)
            results.Add(s_SortedGraphics2[i]);

        s_SortedGraphics2.Clear();
    }
}


public class ReflectionMethodsCache2 {
#if PACKAGE_PHYSICS
 
    public delegate bool Raycast3DCallback(Ray r, out RaycastHit hit, float f, int i);
    public delegate RaycastHit[] RaycastAllCallback(Ray r, float f, int i);
    public delegate int GetRaycastNonAllocCallback(Ray r, RaycastHit[] results, float f, int i);
 
    public Raycast3DCallback raycast3D = null;
    public RaycastAllCallback raycast3DAll = null;
    public GetRaycastNonAllocCallback getRaycastNonAlloc = null;
#endif

#if PACKAGE_PHYSICS2D
 
    public delegate RaycastHit2D Raycast2DCallback(Vector2 p1, Vector2 p2, float f, int i);
    public delegate RaycastHit2D[] GetRayIntersectionAllCallback(Ray r, float f, int i);
    public delegate int GetRayIntersectionAllNonAllocCallback(Ray r, RaycastHit2D[] results, float f, int i);
 
    public Raycast2DCallback raycast2D = null;
    public GetRayIntersectionAllCallback getRayIntersectionAll = null;
    public GetRayIntersectionAllNonAllocCallback getRayIntersectionAllNonAlloc = null;
#endif
    // We call Physics.Raycast and Physics2D.Raycast through reflection to avoid creating a hard dependency from
    // this class to the Physics/Physics2D modules, which would otherwise make it impossible to make content with UI
    // without force-including both modules.
    //
    // *NOTE* If other methods are required ensure to add [RequiredByNativeCode] to the bindings for that function. It prevents
    //        the function from being stripped if required. See Dynamics.bindings.cs for examples (search for GraphicRaycaster.cs).
    public ReflectionMethodsCache2() {
#if PACKAGE_PHYSICS
 
        var raycast3DMethodInfo = typeof(Physics).GetMethod("Raycast", new[] { typeof(Ray), typeof(RaycastHit).MakeByRefType(), typeof(float), typeof(int) });
        if (raycast3DMethodInfo != null)
            raycast3D = (Raycast3DCallback)Delegate.CreateDelegate(typeof(Raycast3DCallback), raycast3DMethodInfo);
 
        var raycastAllMethodInfo = typeof(Physics).GetMethod("RaycastAll", new[] { typeof(Ray), typeof(float), typeof(int) });
        if (raycastAllMethodInfo != null)
            raycast3DAll = (RaycastAllCallback)Delegate.CreateDelegate(typeof(RaycastAllCallback), raycastAllMethodInfo);
 
        var getRaycastAllNonAllocMethodInfo = typeof(Physics).GetMethod("RaycastNonAlloc", new[] { typeof(Ray), typeof(RaycastHit[]), typeof(float), typeof(int) });
        if (getRaycastAllNonAllocMethodInfo != null)
            getRaycastNonAlloc = (GetRaycastNonAllocCallback)Delegate.CreateDelegate(typeof(GetRaycastNonAllocCallback), getRaycastAllNonAllocMethodInfo);
#endif
#if PACKAGE_PHYSICS2D
 
        var raycast2DMethodInfo = typeof(Physics2D).GetMethod("Raycast", new[] { typeof(Vector2), typeof(Vector2), typeof(float), typeof(int) });
        if (raycast2DMethodInfo != null)
            raycast2D = (Raycast2DCallback)Delegate.CreateDelegate(typeof(Raycast2DCallback), raycast2DMethodInfo);
 
        var getRayIntersectionAllMethodInfo = typeof(Physics2D).GetMethod("GetRayIntersectionAll", new[] { typeof(Ray), typeof(float), typeof(int) });
        if (getRayIntersectionAllMethodInfo != null)
            getRayIntersectionAll = (GetRayIntersectionAllCallback)Delegate.CreateDelegate(typeof(GetRayIntersectionAllCallback), getRayIntersectionAllMethodInfo);
 
        var getRayIntersectionAllNonAllocMethodInfo = typeof(Physics2D).GetMethod("GetRayIntersectionNonAlloc", new[] { typeof(Ray), typeof(RaycastHit2D[]), typeof(float), typeof(int) });
        if (getRayIntersectionAllNonAllocMethodInfo != null)
            getRayIntersectionAllNonAlloc = (GetRayIntersectionAllNonAllocCallback)Delegate.CreateDelegate(typeof(GetRayIntersectionAllNonAllocCallback), getRayIntersectionAllNonAllocMethodInfo);
#endif
    }

    private static ReflectionMethodsCache2 s_ReflectionMethodsCache = null;

    public static ReflectionMethodsCache2 Singleton {
        get {
            if (s_ReflectionMethodsCache == null)
                s_ReflectionMethodsCache = new ReflectionMethodsCache2();
            return s_ReflectionMethodsCache;
        }
    }
}
