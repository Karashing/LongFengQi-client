using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using System.Threading;
using System.Linq;
using UnityEngine.Events;
using System.Reflection;

namespace ToolI {
    [Serializable]
    public struct V2Int {
        public int x, y;
        public V2Int(int vx, int vy) {
            x = vx;
            y = vy;
        }
        public static bool operator ==(V2Int u, V2Int v) {
            return u.x == v.x && u.y == v.y;
        }
        public static bool operator !=(V2Int u, V2Int v) {
            return !(u == v);
        }
        public override bool Equals(object obj) {
            if (!(obj is V2Int))
                return false;
            return this == (V2Int)obj;
        }

        public override int GetHashCode() {
            unchecked { // 禁用溢出检查
                int hash = 17;
                hash = hash * 23 + x.GetHashCode();
                hash = hash * 23 + y.GetHashCode();
                return hash;
            }
        }
        public static implicit operator Vector3Int(V2Int v) {
            return new Vector3Int(v.x, v.y);
        }
        public static implicit operator V2Int(Vector3Int v) {
            return new V2Int(v.x, v.y);
        }
    }

    [Serializable]
    public struct RectV3 {
        public float lx, ly, lz, rx, ry, rz;
        public float width {
            get { return rx - lx; }
        }
        public float height {
            get { return ry - ly; }
        }
        public float depth {
            get { return rz - lz; }
        }
        public float midX {
            get { return (lx + rx) / 2; }
        }
        public float midY {
            get { return (ly + ry) / 2; }
        }
        public float midZ {
            get { return (ly + ry) / 2; }
        }
        public RectV3(float leftX, float leftY, float rightX, float rightY) {
            lx = leftX; ly = leftY; lz = 0;
            rx = rightX; ry = rightY; rz = 0;
        }
        public RectV3(float leftX, float leftY, float leftZ, float rightX, float rightY, float rightZ) {
            lx = leftX; ly = leftY; lz = leftZ;
            rx = rightX; ry = rightY; rz = rightZ;
        }
        public RectV3(Vector2 left, Vector2 right) {
            lx = left.x; ly = left.x; lz = 0;
            rx = right.x; ry = right.y; rz = 0;
        }
        public RectV3(Vector3 left, Vector3 right) {
            lx = left.x; ly = left.x; lz = left.x;
            rx = right.x; ry = right.y; rz = right.z;
        }
        public Vector3 Offset(RectV3 rect) {
            Vector3 res = new Vector3(0, 0, 0);
            if (width > rect.width) {
                res.x = midX - rect.midX;
            }
            else {
                if (lx < rect.lx) {
                    res.x = lx - rect.lx;
                }
                else if (rx > rect.rx) {
                    res.x = rx - rect.rx;
                }
            }
            if (height > rect.height) {
                res.y = midY - rect.midY;
            }
            else {
                if (ly < rect.ly) {
                    res.y = ly - rect.ly;
                }
                else if (ry > rect.ry) {
                    res.y = ry - rect.ry;
                }
            }
            if (depth > rect.depth) {
                res.z = midZ - rect.midZ;
            }
            else {
                if (lz < rect.lz) {
                    res.z = lz - rect.lz;
                }
                else if (rz > rect.rz) {
                    res.z = rz - rect.rz;
                }
            }
            return res;
        }
    }

    [Serializable]
    public struct RectV2 {
        public float lx, ly, rx, ry;
        public float width {
            get { return rx - lx; }
        }
        public float height {
            get { return ry - ly; }
        }
        public float midX {
            get { return (lx + rx) / 2; }
        }
        public float midY {
            get { return (ly + ry) / 2; }
        }
        public RectV2(float leftX, float leftY, float rightX, float rightY) {
            lx = leftX; ly = leftY;
            rx = rightX; ry = rightY;
        }
        public RectV2(Vector2 left, Vector2 right) {
            lx = left.x; ly = left.y;
            rx = right.x; ry = right.y;
        }
        public RectV2(Vector3 left, Vector3 right) {
            lx = left.x; ly = left.y;
            rx = right.x; ry = right.y;
        }
        public bool isOffset(RectV2 rect) {
            bool res = true;
            if (width > rect.width) {
                res &= (midX == rect.midX);
            }
            else {
                res &= (rect.lx <= lx && rx <= rect.rx);
            }
            if (height > rect.height) {
                res &= (midY == rect.midY);
            }
            else {
                res &= (rect.ly <= ly && ry <= rect.ry);
            }
            return !res;
        }
        public Vector2 Offset(RectV2 rect) {
            Vector2 res = new Vector2(0, 0);
            if (width > rect.width) {
                res.x = midX - rect.midX;
            }
            else {
                if (lx < rect.lx) {
                    res.x = lx - rect.lx;
                }
                else if (rx > rect.rx) {
                    res.x = rx - rect.rx;
                }
            }
            if (height > rect.height) {
                res.y = midY - rect.midY;
            }
            else {
                if (ly < rect.ly) {
                    res.y = ly - rect.ly;
                }
                else if (ry > rect.ry) {
                    res.y = ry - rect.ry;
                }
            }
            return res;
        }
    }

    [System.Serializable]
    public struct RectV1 {
        public float l, r;
        public float length {
            get { return r - l; }
        }
        public float mid {
            get { return (l + r) / 2; }
        }
        public RectV1(float left, float right) {
            l = left; r = right;
        }
        public RectV1(Vector2 left, Vector2 right) {
            l = left.x; r = right.x;
        }
        public RectV1(Vector3 left, Vector3 right) {
            l = left.x; r = right.x;
        }
        public bool isOffset(RectV1 rect) {
            if (length > rect.length) {
                return !(mid == rect.mid);
            }
            else {
                return !(rect.l <= l && r <= rect.r);
            }
        }
        public float Offset(RectV1 rect) {
            float res = 0;
            if (length > rect.length) {
                res = mid - rect.mid;
            }
            else {
                if (l < rect.l) {
                    res = l - rect.l;
                }
                else if (r > rect.r) {
                    res = r - rect.r;
                }
            }
            return res;
        }
    }

    public static class MathI {
        public static void Swap<T>(ref T lhs, ref T rhs) {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
        private class Romanner {
            public int Num { get; set; }
            public string Roman { get; set; }
        }

        private static List<Romanner> _romanners = new List<Romanner>(){
            new Romanner(){
                Num=1,
                Roman="I"
            },
            new Romanner(){
                Num=4,
                Roman="IV"
            },
            new Romanner(){
                Num=5,
                Roman="V"
            },
            new Romanner(){
                Num=9,
                Roman="IX"
            },
            new Romanner(){
                Num=10,
                Roman="X"
            },
            new Romanner(){
                Num=40,
                Roman="XL"
            },
            new Romanner(){
                Num=50,
                Roman="L"
            },
            new Romanner(){
                Num=90,
                Roman="XC"
            },
            new Romanner(){
                Num=100,
                Roman="C"
            },
            new Romanner(){
                Num=400,
                Roman="CD"
            },
            new Romanner(){
                Num=500,
                Roman="D"
            },
            new Romanner(){
                Num=900,
                Roman="CM"
            },
            new Romanner(){
                Num=1000,
                Roman="M"
            }
        };


        public static string ToRotmanNumbers(int num) {
            // 案例
            // 3 III
            // 4 IV
            // 7 VII
            // 9 IX   10 1
            // 58 LVIII 50 5 1 1 1
            // 299 CCIC
            // 499 ID
            // 1994 MCMXCIV 1000 100 1000 10 100 1 5
            // 2999 MMCMXCIX
            // 472  CDLXXII
            if (num < 0) return "";
            else if (num == 0) return "O";
            
            string sb = "";
            var maxRommaner = _romanners.OrderByDescending(t => t.Num).First();
            while (num > 0) {
                //大于区间范围内的值
                if (num > maxRommaner.Num) {
                    sb += maxRommaner.Roman;
                    num = num - maxRommaner.Num;
                }
                // 在定义区间范围内的值
                else {
                    for (int i = 0; i < _romanners.Count; i++) {
                        // 指定区间
                        if (num == _romanners[i].Num) {
                            sb += _romanners[i].Roman;
                            num = num - _romanners[i].Num;
                            break;
                        }
                        else if (num < _romanners[i].Num) {
                            var previousRomanner = _romanners[i - 1];
                            num = num - previousRomanner.Num;
                            sb += previousRomanner.Roman;
                            break;
                        }
                    }
                }
            }
            return sb;
        }
        public static Vector3 GetRotationFromVector(Vector3 vector, float extra_angle = 0) {
            Vector3 normalizedVector = vector.normalized; // 标准化向量以确保其长度为1
            float angle = Mathf.Atan2(normalizedVector.y, normalizedVector.x) * Mathf.Rad2Deg;// 计算向量与正东方向的夹角
            return new Vector3(0, 0, angle + extra_angle); // 设置物体的z轴旋转角度
        }
    }
    public static class RandomI {
        public static int Range(int min, int max) {
            return UnityEngine.Random.Range(min, max);
        }
    }
    public static class TileMap6 {
        public enum Direct {
            LeftUp = 0,
            RightUp = 1,
            LeftMid = 2,
            RightMid = 3,
            LeftDown = 4,
            RightDown = 5,
            Default = 6,
        }
        public static Direct GetDirect(Vector3Int start_pos, Vector3Int target_pos) {
            if (target_pos.y > start_pos.y) {
                if (start_pos.y % 2 == 0) {
                    if (start_pos.x - 1 == target_pos.x) return Direct.LeftUp;
                    else if (start_pos.x == target_pos.x) return Direct.RightUp;
                }
                else {
                    if (start_pos.x == target_pos.x) return Direct.LeftUp;
                    else if (start_pos.x + 1 == target_pos.x) return Direct.RightUp;
                }
            }
            else if (target_pos.y == start_pos.y) {
                if (start_pos.y % 2 == 0) {
                    if (start_pos.x - 1 == target_pos.x) return Direct.LeftMid;
                    else if (start_pos.x == target_pos.x) return Direct.RightMid;
                }
                else {
                    if (start_pos.x == target_pos.x) return Direct.LeftMid;
                    else if (start_pos.x + 1 == target_pos.x) return Direct.RightMid;
                }

            }
            else {
                if (start_pos.y % 2 == 0) {
                    if (start_pos.x - 1 == target_pos.x) return Direct.LeftDown;
                    else if (start_pos.x == target_pos.x) return Direct.RightDown;
                }
                else {
                    if (start_pos.x == target_pos.x) return Direct.LeftDown;
                    else if (start_pos.x + 1 == target_pos.x) return Direct.RightDown;
                }
            }
            return Direct.Default;
        }
        public static Vector3Int GetAdjacentGrid(Vector3Int grid_position, Direct direct) {
            int x = grid_position.x, y = grid_position.y;
            if (grid_position.y % 2 == 0) {
                switch (direct) {
                    case Direct.LeftUp: return new Vector3Int(x - 1, y + 1); // ↖
                    case Direct.RightUp: return new Vector3Int(x, y + 1); // ↗
                    case Direct.LeftMid: return new Vector3Int(x - 1, y); // ←
                    case Direct.RightMid: return new Vector3Int(x + 1, y); // →
                    case Direct.LeftDown: return new Vector3Int(x - 1, y - 1); // ↙
                    case Direct.RightDown: return new Vector3Int(x, y - 1); // ↘
                    default: return new Vector3Int(x, y);
                }
            }
            else {
                switch (direct) {
                    case Direct.LeftUp: return new Vector3Int(x, y + 1); // ↖
                    case Direct.RightUp: return new Vector3Int(x + 1, y + 1); // ↗
                    case Direct.LeftMid: return new Vector3Int(x - 1, y); // ←
                    case Direct.RightMid: return new Vector3Int(x + 1, y); // →
                    case Direct.LeftDown: return new Vector3Int(x, y - 1); // ↙
                    case Direct.RightDown: return new Vector3Int(x + 1, y - 1); // ↘
                    default: return new Vector3Int(x, y);
                }
            }
        }
        public static List<Vector3Int> GetAdjacentGrids(Vector3Int grid_position) {
            int x = grid_position.x, y = grid_position.y;
            if (grid_position.y % 2 == 0) {
                return new List<Vector3Int>{
                    new Vector3Int(x-1,y+1), // ↖
                    new Vector3Int(x,y+1), // ↗
                    new Vector3Int(x-1,y), // ←
                    new Vector3Int(x+1,y), // →
                    new Vector3Int(x-1,y-1), // ↙
                    new Vector3Int(x,y-1), // ↘
                };
            }
            else {
                return new List<Vector3Int>{
                    new Vector3Int(x,y+1), // ↖
                    new Vector3Int(x+1,y+1), // ↗
                    new Vector3Int(x-1,y), // ←
                    new Vector3Int(x+1,y), // →
                    new Vector3Int(x,y-1), // ↙
                    new Vector3Int(x+1,y-1), // ↘
                };
            }
        }
        public static int GetDistance(Vector3Int g0, Vector3Int g1) {
            if (g0.y > g1.y) MathI.Swap(ref g0, ref g1);
            var dy = Mathf.Abs(g0.y - g1.y);
            var lx = g0.x - (dy + (1 - (Mathf.Abs(g0.y) % 2))) / 2;
            var rx = g0.x + (dy + (Mathf.Abs(g0.y) % 2)) / 2;
            if (lx <= g1.x && g1.x <= rx) {
                return dy;
            }
            else {
                return dy + Mathf.Min(Mathf.Abs(g1.x - lx), Mathf.Abs(g1.x - rx));
            }
        }
        public static List<Vector3Int> GetRangeGrids(Vector3Int start_pos, int range_min = 0, int range_max = 100) {
            var queue = new Queue<Vector3Int>();
            var tags = new HashSet<Vector3Int>();
            var ans = new List<Vector3Int>();
            queue.Enqueue(start_pos);
            tags.Add(start_pos);
            while (queue.Count > 0) {
                var pos = queue.Dequeue();
                var distance = GetDistance(pos, start_pos);
                if (distance >= range_min && distance <= range_max) {
                    ans.Add(pos);
                }
                var adjacents = GetAdjacentGrids(pos);
                foreach (var xpos in adjacents) {
                    if (GetDistance(xpos, start_pos) <= range_max) {
                        if (tags.Add(xpos)) {
                            queue.Enqueue(xpos);
                        }
                    }
                }
            }
            return ans;
        }
        public static List<Vector3Int> GetRayGrids(Vector3Int start_pos, Vector3Int target_pos, int range_min = 0, int range_max = 100) {
            var ans = new List<Vector3Int>();
            var dir = GetDirect(start_pos, target_pos);
            var pos = start_pos;
            for (int i = 0; i <= range_max; ++i) {
                if (i >= range_min) ans.Add(pos);
                pos = GetAdjacentGrid(pos, dir);
            }
            return ans;
        }
    }
    [Serializable]
    public struct KVPair<KT, VT> {
        public KT key;
        public VT value;
    }
    public class ListI<T> : List<T> {
        private UnityAction callback = null;
        public void SetCallback(UnityAction call_back) {
            callback = call_back;
        }
        public void AddI(T item) {
            Add(item);
            if (callback != null)
                callback();
        }
        public void RemoveI(T item) {
            Remove(item);
            if (callback != null)
                callback();
        }
        public void ClearI() {
            Clear();
            if (callback != null)
                callback();
        }
    }
    public class ParasiticDict<KT, VT> : Dictionary<KT, VT> {
        public void LoadBy(List<VT> data_list, Func<VT, KT> getter) {
            foreach (VT element in data_list) {
                Add(getter(element), element);
            }
        }
    }
    public static class RamdonI {
        public static T GetRandomEnum<T>() {
            Array values = Enum.GetValues(typeof(T));
            int randomIndex = UnityEngine.Random.Range(0, values.Length);
            return (T)values.GetValue(randomIndex);
        }
        public static bool RandomTrue(float probability) {
            float randomValue = UnityEngine.Random.Range(0f, 1f);
            return randomValue <= probability;
        }
    }
    public static class JsonI {
        public static T ReadFromJson<T>(string fileName) {
            string filePath = Application.dataPath + "/Resources/Json/" + fileName;
            if (File.Exists(filePath)) {
                StreamReader sr = new StreamReader(filePath);
                string jsonStr = sr.ReadToEnd();
                sr.Close();
                return JsonUtility.FromJson<T>(jsonStr);
            }
            Debug.Log("Json文件加载失败:" + filePath);
            return default(T);
        }
        public static void WriteToJson<T>(T file, string fileName) {
            string filePath = Application.dataPath + "/Resources/Json/" + fileName;
            string jsonStr = JsonUtility.ToJson(file);
            StreamWriter sw = new StreamWriter(filePath);
            sw.Write(jsonStr);
            sw.Close();
            Debug.Log("保存成功:" + filePath);
        }
    }
    public static class TimeI {
        public static string GetTimeStr() {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
    public static class ColorI {
        public static string ToHexString(Color color) {
            return
                ((byte)(color.r * 255)).ToString("X2") +
                ((byte)(color.g * 255)).ToString("X2") +
                ((byte)(color.b * 255)).ToString("X2");
        }
    }
    public static class Generate {
        public class Generator {
            int total_bytes = 64;
            int timestamp_bytes = 41;
            int machine_bytes = 7;
            int sequence_bytes = 10;
            long sequence = 0;
            long machine_id = 0;
            long last_timestamp = -1;
            private long GetCurrentTimestamp() {
                DateTime epochStart = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan timeSpan = DateTime.UtcNow - epochStart;
                return (long)timeSpan.TotalMilliseconds;
            }
            private long WaitForNextTimestamp(long last_timestamp) {
                long timestamp = GetCurrentTimestamp();
                while (timestamp <= last_timestamp) {
                    timestamp = GetCurrentTimestamp();
                }
                return timestamp;
            }
            public string GenerateId() {
                long timestamp = GetCurrentTimestamp();
                if (timestamp < last_timestamp) {
                    Debug.Log("Clock moved backwards. Refusing to generate ID.");
                    return "";
                }
                if (timestamp == last_timestamp) {
                    sequence += 1;
                    while (sequence >= (1 << sequence_bytes)) {
                        timestamp = WaitForNextTimestamp(last_timestamp);
                        sequence = 0;
                    }
                }
                else {
                    sequence = 0;
                }
                last_timestamp = timestamp;
                long generated_id = ((timestamp << (total_bytes - timestamp_bytes))
                                | machine_id << (total_bytes - timestamp_bytes - machine_bytes)
                                | (sequence));
                return generated_id.ToString();
            }
        }
        public static Generator generator = new Generator();
        public static string GenerateId() {
            return generator.GenerateId();
        }
    }
}
namespace UnityEngine {
#if UNITY_EDITOR
    using UnityEditor;
#endif


    public class ReadOnlyAttribute : PropertyAttribute {

    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
}