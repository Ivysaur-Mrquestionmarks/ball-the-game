using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;

public class ScoresView : MonoBehaviour
{

    public string level;
    public TextMeshProUGUI Text;
    //Note, it has to be the level scene it leads to, cuz I made a mistake.
    //;-;

    // Start is called before the first frame update
    void Start()
    {
        string destination = Application.persistentDataPath + level + ".ball";
        FileStream file;
        if (File.Exists(destination))
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.OpenRead(destination);
            levelRecords Records = (levelRecords)bf.Deserialize(file);
            Text.text = "Best time: " + Records.BestTime + "\nHigest score: " + Records.BestScore + "\nTime of best score: " + Records.BestTimeScore;
        }
        else {
            Text.text = "No record yet";
        }
    }


}
