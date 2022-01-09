using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
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
    }
}
