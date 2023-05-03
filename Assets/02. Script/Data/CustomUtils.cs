using UnityEngine;
using System.Net.NetworkInformation;
using System;
using System.Collections.Generic;

namespace StartPage
{

    public static class CustomUtils
    {
        public static string GetMacAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                Debug.Log(
                    "Found MAC Address: " + nic.GetPhysicalAddress() +
                    " Type: " + nic.NetworkInterfaceType);
                if (nic.NetworkInterfaceType.Equals(NetworkInterfaceType.Ethernet))
                {
                    return nic.GetPhysicalAddress().ToString();
                }
            }

            return "";
        }

        public static T[] ShuffleArray<T>(T[] array, int seed = -1)
        {
            System.Random rand;
            if (seed == -1)
            {
                rand = new System.Random();
            }
            else
            {
                rand = new System.Random(seed);
            }

            for (int i = 0; i < array.Length - 1; i++)
            {
                int randIndex = rand.Next(i, array.Length);
                T temp = array[randIndex];
                array[randIndex] = array[i];
                array[i] = temp;
            }

            return array;
        }


        public static string GetTodayStr()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");// + UnityEngine.Random.Range(10, 30)
        }

        //public static string GetIP()
        //{
        //    return Network.player.ipAddress;
        //}


        public static UnityEngine.Gradient GradientLerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return gradientLerp(a, b, t, false, false);
        }

        public static UnityEngine.Gradient GradientLerpNoAlpha(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return gradientLerp(a, b, t, true, false);
        }

        public static UnityEngine.Gradient GradientLerpNoColor(UnityEngine.Gradient a, UnityEngine.Gradient b, float t)
        {
            return gradientLerp(a, b, t, false, true);
        }

        static UnityEngine.Gradient gradientLerp(UnityEngine.Gradient a, UnityEngine.Gradient b, float t, bool noAlpha, bool noColor)
        {
            //list of all the unique key times
            var keysTimes = new List<float>();

            if (!noColor)
            {
                for (int i = 0; i < a.colorKeys.Length; i++)
                {
                    float k = a.colorKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }

                for (int i = 0; i < b.colorKeys.Length; i++)
                {
                    float k = b.colorKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }
            }

            if (!noAlpha)
            {
                for (int i = 0; i < a.alphaKeys.Length; i++)
                {
                    float k = a.alphaKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }

                for (int i = 0; i < b.alphaKeys.Length; i++)
                {
                    float k = b.alphaKeys[i].time;
                    if (!keysTimes.Contains(k))
                        keysTimes.Add(k);
                }
            }

            GradientColorKey[] clrs = new GradientColorKey[keysTimes.Count];
            GradientAlphaKey[] alphas = new GradientAlphaKey[keysTimes.Count];

            //Pick colors of both gradients at key times and lerp them
            for (int i = 0; i < keysTimes.Count; i++)
            {
                float key = keysTimes[i];
                var clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
                clrs[i] = new GradientColorKey(clr, key);
                alphas[i] = new GradientAlphaKey(clr.a, key);
            }

            var g = new UnityEngine.Gradient();
            g.SetKeys(clrs, alphas);

            return g;
        }



        public static Vector2 ToXZ(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }
    }


    [System.Serializable]
    public class RandValue
    {
        [SerializeField]
        public float min;
        [SerializeField]
        public float max;

        public float Value;

        public float SetRandomValue()
        {
            Value = UnityEngine.Random.Range(min, max);
            return Value;
        }
    }

    [System.Serializable]
    public class RandPosition
    {
        [SerializeField]
        private Transform[] PointList;
        private int index;

        public Vector3 SetRandomPoint()
        {
            index = UnityEngine.Random.Range(0, PointList.Length);
            return PointList[index].position;
        }

        public Vector3 GetPoint()
        {
            return PointList[index].position;
        }
    }


}