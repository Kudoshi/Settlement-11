using UnityEngine;

public static class FireflyManager
{
    private static int fireflyQty = 0;
    private static int fireflyMaxQty = 25;

    public static int FireflyQty => fireflyQty;

    public static void CollectFirefly()
    {
        if (fireflyQty < fireflyMaxQty)
            fireflyQty++;
    }

    // Skills
    public static void FirefliesDash()
    {
        if (fireflyQty >= 10)
            fireflyQty -= 10;
    }

    public static void FirefliesSpin()
    {
        if (fireflyQty >= 20)
            fireflyQty -= 20;
    }
}