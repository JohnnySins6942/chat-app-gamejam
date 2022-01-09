using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System;

namespace Michsky.UI.ModernUIPack
{
    public class ChatManager : MonoBehaviour
    {
        public Authentication auth;
        private FirebaseUser user;

        [Header("UI")]
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI bioText;
        public GameObject profileTab;
        public TMP_InputField friendInput;
        public GameObject friendsTab;

        public List<System.Object> chats = new List<System.Object>();
        public GameObject chatPrefab;
        public Transform viewport;
        public void SpawnChat()
        {
            DocumentReference docRef = FirebaseFirestore.DefaultInstance.Collection("userChats").Document(auth.user.UserId);
            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Dictionary<string, object> city = snapshot.ToDictionary();
                    foreach (KeyValuePair<string, object> pair in city)
                    {
                        chats.Add(pair.Key.ToString());
                    }
                    print(chats.Count);
                    foreach (var item in chats)
                    {
                        var v = Instantiate(chatPrefab, chatPrefab.transform.position, chatPrefab.transform.rotation);
                        v.transform.SetParent(viewport);
                        var b = v.GetComponent<ChatObject>();
                        v.transform.localScale = new Vector3(1, 1, 1);
                        b.id = item.ToString();
                        b.findName();
                    }
                }
            });
        }
           
        public void transfer()
        {
            if(auth.user != null)
            {
                user = auth.user;
                print(auth.user.DisplayName);
                print(user.DisplayName);
            }
        }
        private void Start()
        {
            transfer();
        }
        public void closeProfile()
        {
            profileTab.SetActive(false);
        }
        public void OpenFriend()
        {
            friendsTab.SetActive(true);
        }
        public void CloseFriends()
        {
            friendsTab.SetActive(false);
        }
        public void OpenProfile()
        {
            transfer();
            profileTab.SetActive(true);
            nameText.text = user.DisplayName;
        }
        public void signOut()
        {
            auth.SignOut();
            closeProfile();
        }
        public void AddFriend()
        {
            if (friendInput.text != "" && friendInput.text != " ")
            {
                var firestore = FirebaseFirestore.DefaultInstance;
                CollectionReference userRef = firestore.Collection("userProfiles");
                Query query = userRef.WhereEqualTo("name", friendInput.text);
                query.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
                {
                    if (querySnapshotTask.Result.Documents != null)
                    {
                        foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
                        {

                            var firestore = FirebaseFirestore.DefaultInstance;
                            Dictionary<string, object> userData = new Dictionary<string, object>
                            {
                                    { user.UserId, new List<object>() }
                            };
                            Dictionary<string, object> userData2 = new Dictionary<string, object>
                            {
                                    { documentSnapshot.Id, new List<object>() }
                            };
                            FirebaseFirestore.DefaultInstance.Collection("userChats").Document(documentSnapshot.Id).SetAsync(userData, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
                            {
                                if (task.IsCompleted)
                                {
                                    FirebaseFirestore.DefaultInstance.Collection("userChats").Document(user.UserId).SetAsync(userData2, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
                                    {
                                        if (task.IsCompleted)
                                        {
                                        }
                                        else
                                        {
                                            print(task.Exception);
                                        }
                                    });
                                }
                                else
                                {
                                    print(task.Exception);
                                }
                            });
                        }
                    }
                });
            }
        }
    }
}
