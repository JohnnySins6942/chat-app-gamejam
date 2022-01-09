using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using TMPro;
using Firebase.Extensions;

public class ChatObject : MonoBehaviour
{
    public string id;
    public TextMeshProUGUI text;
    public new string name;
    public void findName()
    {
        DocumentReference docRef = FirebaseFirestore.DefaultInstance.Collection("userProfiles").Document(id);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Dictionary<string, object> var = snapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in var)
                {
                    if(pair.Key == "name")
                    {
                        name = pair.Value.ToString();
                    }
                }
                text.text = name;
            }
        });
    }

}
