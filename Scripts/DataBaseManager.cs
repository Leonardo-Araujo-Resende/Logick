using UnityEngine;
using Firebase;
using Firebase.Database;

public class DataBaseManager : MonoBehaviour
{
    DatabaseReference dBreference;
    string userID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void criarEstatistica(NivelStatistics nivelStatistics)
    {
        string json = JsonUtility.ToJson(nivelStatistics);
        dBreference.Child("estatisticas").Push().SetRawJsonValueAsync(json);
    }
}
