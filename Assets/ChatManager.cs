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
                            Dictionary<string, object> data = documentSnapshot.ToDictionary();
                            data.Add("notification", user.UserId);
                            print(data);
                            FirebaseFirestore.DefaultInstance.Collection("userProfiles").Document(user.UserId).SetAsync(data).ContinueWithOnMainThread(task =>
                            {
                                if (task.IsCompleted)
                                {
                                    print("added");
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
