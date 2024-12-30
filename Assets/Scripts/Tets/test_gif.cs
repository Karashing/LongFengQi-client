

// using System.Collections.Generic;
// using System.Drawing;
// using System.Drawing.Imaging;

// using UnityEngine;

// public class AnimatedGifDrawer : MonoBehaviour
// {
//     public string loadingGifPath;//路径
//     public UITexture tex;//图片
//     public float speed = 0.1f;//播放速度

//     private bool isPlay = false;//是否播放
//     private int i = 0;//控制要播放的图片

//     private List<Texture2D> gifFrames = new List<Texture2D>();//存储解析出来的图片
//     void Awake()
//     {
//         Image gifImage = Image.FromFile(loadingGifPath);
//         FrameDimension dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
//         int frameCount = gifImage.GetFrameCount(dimension);
//         for (int i = 0; i < frameCount; i++)
//         {
//             gifImage.SelectActiveFrame(dimension, i);
//             Bitmap frame = new Bitmap(gifImage.Width, gifImage.Height);
//             System.Drawing.Graphics.FromImage(frame).DrawImage(gifImage, Point.Empty);
//             Texture2D frameTexture = new Texture2D(frame.Width, frame.Height);
//             for (int x = 0; x < frame.Width; x++)
//                 for (int y = 0; y < frame.Height; y++)
//                 {
//                     System.Drawing.Color sourceColor = frame.GetPixel(x, y);
//                     frameTexture.SetPixel(x, frame.Height - 1 - y, new Color32(sourceColor.R, sourceColor.G, sourceColor.B, sourceColor.A)); // for some reason, x is flipped
//                 }
//             frameTexture.Apply();
//             gifFrames.Add(frameTexture);
//         }
//     }

//     private void Update()
//     {
//         if (isPlay == true)
//         {
//             i++;
//             tex.mainTexture = gifFrames[(int)(i * speed) % gifFrames.Count];
//         }
//     }

//     /// <summary>
//     /// 播放动画
//     /// </summary>
//     public void StartAni()
//     {
//         isPlay = true;
//     }

//     /// <summary>
//     /// 停止动画
//     /// </summary>
//     public void StopAni()
//     {
//         isPlay = false;
//         i = 0;
//     }
// }