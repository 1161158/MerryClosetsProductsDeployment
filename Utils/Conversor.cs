using System.Collections.Generic;
using System.Linq;

public class Conversor{

    public static float MeterToCentimeter (float meter){
        return meter * 100;
    }
    public static float MeterToDecimeter (float meter){
        return meter *10;
    }
    public static float MeterToMilimeter (float meter){
        return meter * 1000;
    }
    public static float CentimeterToMeter (float centimeter){
        return centimeter * 0.01f;
    }
    public static float CentimeterToDecimeter  (float centimeter){
        return centimeter * 0.1f;
    }
    public static float CentimeterToMilimeter (float centimeter){
        return centimeter * 10;
    }

    public static float DecimeterToMeter (float decimeter){
        return decimeter * 10;
    }
    public static float DecimeterToCentimeter  (float decimeter){
        return decimeter * 10;
    }
    public static float DecimeterToMilimeter (float decimeter){
        return decimeter * 100;
    }

    public static float MilimeterToMeter (float milimeter){
        return milimeter * 0.001f;
    }
    public static float MilimeterToCentimeter  (float milimeter){
        return milimeter * 0.1f;
    }
    public static float MilimeterToDecimeter (float milimeter){
        return milimeter * 0.01f;
    }
}