using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunManager : MonoBehaviour
{
    public SunManagerData Data;

    [Header("")]
    public bool IsNow = false;
    public int Year = 2020;
    [Range(1, 12)]
    public int Month = 6;
    [Range(1, 31)]
    public int Day = 22;
    [Range(0, 24)]
    public int Hour = 12;
    [Range(0, 59)]
    public int Min = 0;
    [Range(0, 59)]
    public int Sec = 0;

    [Header("Data")]
    public int day_of_year;
    public float eot;

    [Header("")]
    public GameObject HourAngleObject;
    public float hour_angle;
    public GameObject SolarDeclination;
    public float solar_declination;
    public float solar_altitude;
    public float solar_azimuth;
    public float solar_azimuth2;
    public float solar_zenith_angle;


    private void Start()
    {
        Load();
    }

    public void Save()
    {

    }
    public void Load()
    {
        Data = new SunManagerData();

        CalculateDay();
    }



    private void OnValidate()
    {
        Ca();
        //CalculateDay();
        //Calculate();
    }
    [ContextMenu("Day와 관련된 계산")]
    private void CalculateDay()
    {
        //균시차(eot) 구하기 
        day_of_year = new DateTime(Year, Month, Day).DayOfYear;
        var b = (day_of_year - 1) * 360.0 / 365.0;
        eot = (float)(229.2 * (0.000075
            + 0.001868 * Math.Cos(Mathf.Deg2Rad * b)
            - 0.032077 * Math.Sin(Mathf.Deg2Rad * b)
            - 0.014615 * Math.Cos(Mathf.Deg2Rad * 2 * b)
            - 0.04089 * Math.Sin(Mathf.Deg2Rad * 2 * b)));


        //태양 적위 구하기
        // 태양의 중싱과 지구의 중심을 연결하는 선과 지구 적도면이 이루는 각
        // 북반구 + 
        // 남반구 -
        solar_declination = (float)(23.45 * Math.Sin(Mathf.Deg2Rad * 360.0 / 365 * (284 + day_of_year)));
    }

    [ContextMenu("계산")]
    private void Calculate()
    {
        //시간각 구하기
        //정남향 0도 기준
        //동쪽 -
        //서쪽 +
        var local_hour_decimal = Hour + (Min + Data.OffsetMinute) / 60.0f + Sec / 3600f;
        var delta_longitude = Data.LocalLongitude - Data.StandrdLongitude;
        hour_angle = (local_hour_decimal * 60.0f + 4 * delta_longitude + eot) / 60.0f * 15 - 180;
        HourAngleObject.transform.localEulerAngles = new Vector3(0, 0, hour_angle);


        //태양 고도 구하기
        //태양과 지표면이 이루는 각. 결국 태양이 어라나 높이 떠있는지 알기 위한 각도
        var term_1 =
            Math.Cos(Mathf.Deg2Rad * Data.LocalLatitude)
            * Math.Cos(Mathf.Deg2Rad * solar_declination)
            * Math.Cos(Mathf.Deg2Rad * hour_angle)
            + Math.Sin(Mathf.Deg2Rad * Data.LocalLatitude)
            * Math.Sin(Mathf.Deg2Rad * solar_declination);
        solar_altitude = (float)(Mathf.Rad2Deg * Math.Asin(term_1));
        //SolarDeclination.transform.localEulerAngles = new Vector3(solar_altitude, 0, 0);

        //태양 방위각 구하기
        //태양과 남향이 이루는 각도
        // 남향을 기준으로
        // 동쪽에 있으면 +
        // 서쪽에 있으면 -
        var term_2 =
            (Math.Sin(Mathf.Deg2Rad * solar_altitude)
            * Math.Sin(Mathf.Deg2Rad * Data.LocalLatitude)
            - Math.Sin(Mathf.Deg2Rad * solar_declination))
            / (Math.Cos(Mathf.Deg2Rad * solar_altitude)
                * Math.Cos(Mathf.Deg2Rad * Data.LocalLatitude)
            );
        solar_azimuth = (float)(Mathf.Rad2Deg * Math.Acos(term_2));




        solar_zenith_angle = (float)(Mathf.Rad2Deg
            * Math.Acos(
                Math.Sin(Mathf.Deg2Rad * Data.LocalLatitude)
                * Math.Sin(Mathf.Deg2Rad * solar_declination)
                + Math.Cos(Mathf.Deg2Rad * Data.LocalLatitude)
                * Math.Cos(Mathf.Deg2Rad * solar_declination)
                * Math.Cos(Mathf.Deg2Rad * hour_angle)
            ));


        var term_3 = (
            Math.Sin(Mathf.Deg2Rad * solar_declination)
            * Math.Cos(Mathf.Deg2Rad * Data.LocalLatitude)
            - (Math.Cos(Mathf.Deg2Rad * hour_angle)
            * Math.Cos(Mathf.Deg2Rad * solar_declination)
            * Math.Sin(Mathf.Deg2Rad * Data.LocalLatitude)))
            /// Math.Sin(Mathf.Deg2Rad * (90 - solar_altitude));
            / Math.Sin(Mathf.Deg2Rad * solar_zenith_angle);

        solar_azimuth2 = (float)(Mathf.Rad2Deg * Math.Acos(term_3));

        transform.localEulerAngles = new Vector3(0, Data.Rotation, 0);
    }

    private int beforeSec = -1;
    private void Update()
    {
        if (IsNow)
        {
            var now = DateTime.Now;
            Year = now.Year;
            Month = now.Month;
            Day = now.Day;
            Hour = now.Hour;
            Min = now.Minute;
            Sec = now.Second;



            ////분이 바뀌면
            //if(CurrentMin != Min)
            //{
            //    CurrentHour = Hour;
            //    CurrentMin = Min;
            //    TargetHour = Hour;
            //    TargetMin = (CurrentMin + 1) % 60;
            //    if (TargetMin == 0)
            //    {
            //        TargetHour = Hour + 1;
            //    }
            //}
        }
        if(beforeSec != Sec)
        {
            beforeSec = Sec;
            //Calculate();
            Ca();
        }
    }




    void Ca()
    {
        float d = 367 * Year - 7 * (Year + (Month / 12 + 9) / 12) / 4 + 275 * Month / 9 + Day - 730530;
        internalHour = Hour + ((Min + Data.OffsetMinute) * 0.0166667f) + (Sec * 0.000277778f);

        d += (GetUniversalTimeOfDay() / 24f);

        float ecl = 23.4393f - 3.563E-7f * d;

        CalculateSunPosition(d, ecl);
    }


    public Light Sun;

    public Vector3 OrbitalToLocal(float theta, float phi)
    {
        Vector3 pos;

        float sinTheta = Mathf.Sin(theta);
        float cosTheta = Mathf.Cos(theta);
        float sinPhi = Mathf.Sin(phi);
        float cosPhi = Mathf.Cos(phi);

        pos.z = sinTheta * cosPhi;
        pos.y = cosTheta;
        pos.x = sinTheta * sinPhi;

        return pos;
    }

    public void CalculateSunPosition(float d, float ecl)
    {
        /////http://www.stjarnhimlen.se/comp/ppcomp.html#5////
        ///////////////////////// SUN ////////////////////////
        float w = 282.9404f + 4.70935E-5f * d;
        float e = 0.016709f - 1.151E-9f * d;
        float M = 356.0470f + 0.9856002585f * d;

        float E = M + e * Mathf.Rad2Deg * Mathf.Sin(Mathf.Deg2Rad * M) * (1 + e * Mathf.Cos(Mathf.Deg2Rad * M));

        float xv = Mathf.Cos(Mathf.Deg2Rad * E) - e;
        float yv = Mathf.Sin(Mathf.Deg2Rad * E) * Mathf.Sqrt(1 - e * e);

        float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
        float r = Mathf.Sqrt(xv * xv + yv * yv);

        float l = v + w;

        float xs = r * Mathf.Cos(Mathf.Deg2Rad * l);
        float ys = r * Mathf.Sin(Mathf.Deg2Rad * l);

        float xe = xs;
        float ye = ys * Mathf.Cos(Mathf.Deg2Rad * ecl);
        float ze = ys * Mathf.Sin(Mathf.Deg2Rad * ecl);

        float decl_rad = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));
        float decl_sin = Mathf.Sin(decl_rad);
        float decl_cos = Mathf.Cos(decl_rad);

        float GMST0 = (l + 180);
        float GMST = GMST0 + GetUniversalTimeOfDay() * 15;
        var LST = GMST + Data.LocalLongitude;

        float HA_deg = LST - Mathf.Rad2Deg * Mathf.Atan2(ye, xe);
        float HA_rad = Mathf.Deg2Rad * HA_deg;
        float HA_sin = Mathf.Sin(HA_rad);
        float HA_cos = Mathf.Cos(HA_rad);

        float x = HA_cos * decl_cos;
        float y = HA_sin * decl_cos;
        float z = decl_sin;

        float sin_Lat = Mathf.Sin(Mathf.Deg2Rad * Data.LocalLatitude);
        float cos_Lat = Mathf.Cos(Mathf.Deg2Rad * Data.LocalLatitude);

        float xhor = x * sin_Lat - z * cos_Lat;
        float yhor = y;
        float zhor = x * cos_Lat + z * sin_Lat;

        float azimuth = Mathf.Atan2(yhor, xhor) + Mathf.Deg2Rad * 180;
        float altitude = Mathf.Atan2(zhor, Mathf.Sqrt(xhor * xhor + yhor * yhor));

        float sunTheta = (90 * Mathf.Deg2Rad) - altitude;
        float sunPhi = azimuth;

        //Set SolarTime: 1 = mid-day (sun directly above you), 0.5 = sunset/dawn, 0 = midnight;
        //GameTime.solarTime = Mathf.Clamp01(Remap(sunTheta, -1.5f, 0f, 1.5f, 1f));

        Sun.transform.localPosition = OrbitalToLocal(sunTheta, sunPhi);
        Sun.transform.LookAt(transform.position);
    }

    public float internalHour;
    /// <summary>
    /// Get current time in hours. UTC0 (12.5 = 12:30)
    /// </summary>
    /// <returns>The the current time of day in hours.</returns>
    public float GetUniversalTimeOfDay()
    {
        return internalHour - 9;// - GameTime.utcOffset;
    }
}

[System.Serializable]

public class SunManagerData
{
    public float LocalLatitude = 37.478f; //35.8393
    public float LocalLongitude = 127.148f; //128.4877
    public int StandrdLongitude = 135;
    public float Rotation;
    public int OffsetMinute = 0;
}
