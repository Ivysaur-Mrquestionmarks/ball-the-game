[System.Serializable]
public class levelRecords
{
    public float BestTime, BestTimeScore;
    public int BestScore;

    public levelRecords(float bestTime_, float BestTimeScore_, int BestScore_)
    {
        BestTime = bestTime_;
        BestTimeScore = BestTimeScore_;
        BestScore = BestScore_;
    }
}
