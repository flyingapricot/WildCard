using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoginPage : MonoBehaviour

{
    public GameObject email;
    public GameObject password;
    public Button login;
    public Button create;
    public Button resetPass;
    public GameObject login_Text;
    //public GameObject setUsernamePage;
    public GameObject doLoginPage;
    //public TMP_Text usernameText;
    private const string MONGO_URI = "mongodb+srv://testUser:testPassword@cluster0.ksmmfvb.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
    private const string DATABASE_NAME = "WildCard";
    private MongoClient client;
    private IMongoDatabase db;
    IMongoCollection<BsonDocument> collection;
    // Start is called before the first frame update
    void Start()
    {
        client = new MongoClient(MONGO_URI);
        db = client.GetDatabase(DATABASE_NAME);
        collection = db.GetCollection<BsonDocument>("users");
        login.onClick.AddListener(doLogin);
        create.onClick.AddListener(doCreate);
        resetPass.onClick.AddListener(doReset);
        doLoginPage.SetActive(true);
        //setUsernamePage.SetActive(false);
    }


/*    public async void updateUsername()
    {
        var filter = Builders<BsonDocument>.Filter.Eq("email", email);
        //var update = Builders<BsonDocument>.Update.Set("username", usernameText.text);

        var result = await collection.UpdateOneAsync(filter, update);

    }*/
    public async void createUser(string email, string password)
    {
        var userDetails = new BsonDocument { { "email", email }, { "password", password }, { "username", "Player" } };
        collection.InsertOne(userDetails);
    }

    public async void checkDetails(string email, string password)
    {
        //Check if user exists
        var filter = Builders<BsonDocument>.Filter.Eq("email", email);
        var result = await collection.Find(filter).FirstOrDefaultAsync();
        if (result != null)
        {
            // User found, check if password matches
            var password_fromServer = result["password"].AsString;
            if (password_fromServer == password)
            {
                Debug.Log($"User with email {email} exists and password matches.");
                login_Text.GetComponent<TMP_Text>().text = "Successful Login";
                PlayerInfo.getInstance().setEmail(email);
                
                //Retrieve username
                if (result["username"] == null)
                {
                    //Set to empty
                    PlayerInfo.getInstance().setUsername("null");
                }
                else
                {
                    PlayerInfo.getInstance().setUsername(result["username"].AsString);
                }
                //Move to username screen
                Debug.Log(PlayerInfo.getInstance().getUsername());
                //setUsernamePage.SetActive(true);
                //doLoginPage.SetActive(false);
            }
            else
            {
                Debug.Log($"User with email {email} exists but password does not match.");
                login_Text.GetComponent<TMP_Text>().text = "Incorrect details. Reset email/password?";
            }
        }
        else
        {
            Debug.Log($"User with email {email} does not exist.");
            login_Text.GetComponent<TMP_Text>().text = "No account with email. Create user?";
        }
    }


    async void doCreate()
    {
        //Check if user exists
        var filter = Builders<BsonDocument>.Filter.Eq("email", email.GetComponent<TMP_InputField>().text);
        var result = await collection.Find(filter).FirstOrDefaultAsync();
        createUser(email.GetComponent<TMP_InputField>().text, password.GetComponent<TMP_InputField>().text);
        login_Text.GetComponent<TMP_Text>().text = "Account Created, login now.";
    }

    void doReset()
    {

    }

    void doLogin()
    {
        checkDetails(email.GetComponent<TMP_InputField>().text, password.GetComponent<TMP_InputField>().text);
        /*        Debug.Log(email.GetComponent<TMP_InputField>().text);
                Debug.Log(password.GetComponent<TMP_InputField>().text);*/
        /*        var document = new BsonDocument { { "email", "username@gmail.com" }, { "password", "abcd1234" } };
                collection.InsertOne(document);*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
