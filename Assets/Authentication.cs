using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase;
using TMPro;
namespace Michsky.UI.ModernUIPack
{
    public class Authentication : MonoBehaviour
    {
        [Header("Firebase")]
        public DependencyStatus dependencyStatus;
        public FirebaseAuth auth;
        public FirebaseUser user;

        [Header("Login")]
        public TMP_InputField emailLoginField;
        public TMP_InputField passwordLoginField;
        public TMP_Text warningText;

        [Header("Register")]
        public TMP_InputField usernameRegisterField;
        public TMP_InputField emailRegisterField;
        public TMP_InputField passwordRegisterField;
        public TMP_InputField passwordRegisterVerifyField;
        public TMP_Text warningRegisterText;

        public WindowManager manager;

        private bool notIn = true;


        private void Awake()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase Dependencies: " + dependencyStatus);
                }
            });
        }
        private void Update()
        {
            if (notIn)
            {

            }
        }
        private void Start()
        {           
        }
        private void InitializeFirebase()
        {
            auth = FirebaseAuth.DefaultInstance;
            if (auth.CurrentUser != null)
            {
                user = auth.CurrentUser;
            }
        }
        public void LoginButton()
        {
            StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        }
        public void RegisterButton()
        {
            StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
        }

        private IEnumerator Login(string _email, string _password)
        {
            var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

            if (LoginTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Login Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WrongPassword:
                        message = "Wrong Password";
                        break;
                    case AuthError.InvalidEmail:
                        message = "Invalid Email";
                        break;
                    case AuthError.UserNotFound:
                        message = "Account does not exist";
                        break;
                }
                warningText.text = message;
            }
            else
            {
                //user is now logged in
                user = LoginTask.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
                warningText.text = "";
                manager.OpenPanel("HomePanel");
            }
        }
        private IEnumerator Register(string _email, string _password, string _username)
        {
            if (_username == "" || _username == " ")
            {
                warningRegisterText.text = "Missing Username";
            }
            else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
            {
                warningRegisterText.text = "Passwords do not match!";
            }
            else
            {
                var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
                yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

                if (RegisterTask.Exception != null)
                {
                    Debug.LogWarning(message: $"failed to register task with {RegisterTask.Exception}");
                    FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                    string message = "Register Failed!";
                    switch (errorCode)
                    {
                        case AuthError.MissingEmail:
                            message = "Missing Email!";
                            break;
                        case AuthError.MissingPassword:
                            message = "Missing Password!";
                            break;
                        case AuthError.WeakPassword:
                            message = "Weak Password!";
                            break;
                        case AuthError.EmailAlreadyInUse:
                            message = "Email Already in use!";
                            break;
                    }
                    warningRegisterText.text = message;
                }
                else
                {
                    user = RegisterTask.Result;
                    if (user != null)
                    {
                        UserProfile profile = new UserProfile { DisplayName = _username };

                        var ProfileTask = user.UpdateUserProfileAsync(profile);

                        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                        if (ProfileTask.Exception != null)
                        {
                            print(message: $"Failed to register task with {ProfileTask.Exception}");
                            FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                            warningRegisterText.text = "Username Set Failed!";
                        }
                        else
                        {
                            warningRegisterText.text = "";
                            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
                            manager.OpenPanel("HomePanel");
                        }
                    }
                }
            }
        }
    }

}
