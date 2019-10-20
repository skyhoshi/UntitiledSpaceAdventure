using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace StageShooter
{
    [System.Serializable]
    public class ShooterSettings
    {
        [UnityEngine.SerializeField]
        public float LevelDuration = 12;

        [UnityEngine.SerializeField]
        public float EnemySpawnTime = 3;

        [UnityEngine.SerializeField]
        public int StartingLives = 6;

        public Transform[] EnemyPrefabList;
    }

    /// <summary>
    /// Stage shooter object. Determins when the level is completed
    /// </summary>
    public class LevelController : MonoBehaviour 
    {
        /// <summary>
        /// Stage settings object
        /// </summary>
        public ShooterSettings[] StageSettings;

        /// <summary>
        /// 
        /// </summary>
        [Tooltip("Flash colour")]
        public Transform PlayerPrefab;
        public Transform[] EnemyPrefabList;
        public Transform MainCanvas;
        public ShooterSettings mSettings;
        public bool SpawnPlayerOnStart = true;
        private float mGameDuration;

        private int mScore = 0;
        public int PlayerScore
        {
            get { return mScore; }

        }

        public void AddScore(int _value)
        {
            if (_value < 0)
                Debug.LogError("LevelController:Add Score: value is negative");

            mScore += _value;

            if (PlayerScoreChanged != null)
                PlayerScoreChanged(this, System.EventArgs.Empty);
        }

        private bool mGameStarted = false;
        public bool GameStarted
        {
            get { return mGameStarted; } 
        }

        private Destructable mPlayer;
        private int mNumLives;
        public int NumLives
        {
            get { return mNumLives; }
        }

        private bool bEnded = false;

        #region Events

        public float RespawnTime = 1.5f;
        
        // Triggered when the game is paused
        public event System.EventHandler GamePausedEvent;
        // Triggered when the game is resumed
        public event System.EventHandler GameResumedEvent;

        // Triggered when player dies
        public event System.EventHandler PlayerDeadEvent;

        // Triggered when player is respawned
        public event System.EventHandler PlayerRespawnEvent;

        // Triggered when looses
        public event System.EventHandler PlayerLoose;

        // Triggered when looses
        public event System.EventHandler PlayerWin;

        // Triggered when player dies
        public event System.EventHandler PlayerScoreChanged;
        #endregion

        #region EnableEtc
        void OnEnable()
        {
            if (mGameStarted)
                StartCoroutine(GameTimer());
        }

        void OnDisable()
        {

        }

        void OnValidate()
        {

        }

        void Start()
        {
            StartGame();
        }
        #endregion

        void StartGame()
        {
            mNumLives = mSettings.StartingLives;
            
            //if (SpawnPlayerOnStart == true)
                SpawnPlayer();
            
            if (mEnemies == null)
                mEnemies = new List<Destructable>();

            GameObject[] EnemieGameObjects = GameObject.FindGameObjectsWithTag("Enemy");
            Destructable e;
            // TODO: Split the shooter types code from here. Enemy Vs TacticalEnemy. Destructable is common between the types.
            foreach (GameObject enemy in EnemieGameObjects)
            {
                e = enemy.GetComponent<Destructable>();
                e.DeadEvent += new Destructable.Destroyed(EnemyDiedEvent);
                mEnemies.Add(e);
            }

            //return;
            StartCoroutine(GameTimer());
        }

        void SpawnPlayer()
        {
            if (mPlayer != null)
                throw new System.Exception("Player already exists");

            GameObject t = GameObject.FindGameObjectWithTag("Player");

            if (t == null)
            {
                // Any other relevent stuff goes here
                t = ((Transform)Instantiate(PlayerPrefab, transform.position, Quaternion.identity)).gameObject;
            }
            else
                Debug.Log("Have a player");      

            mPlayer = t.GetComponent<Destructable>();

            Destructable mPlayerDestructable = (Destructable)t.gameObject.GetComponent<Destructable>();
            Debug.Log("Resgistering event");
            mPlayerDestructable.DeadEvent += new Destructable.Destroyed(mPlayer_PlayerDeadEvent);
            Debug.Log("event registered");
            Player p= (Player)t.gameObject.GetComponent<Player>();

            if (p != null)
                p.mainCanvas = MainCanvas;
            

            if (PlayerRespawnEvent != null)
                PlayerRespawnEvent(this, System.EventArgs.Empty);
        }

        void mPlayer_PlayerDeadEvent(System.EventArgs _damageEventArgs)
        {
            Debug.Log("LevelController: Player died");
            mPlayer = null;
            mNumLives--;

            try
            {   // Executing this in a try catch because code not part of this object may break and we need to continue execution in this function
                if (PlayerDeadEvent != null)
                    PlayerDeadEvent(this, System.EventArgs.Empty);
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
            }

            // If the player is out of lives stop the game
            if (mNumLives <= 0)
            {
                bEnded = true; // Stops the game

                if (PlayerLoose != null)
                    PlayerLoose(this, System.EventArgs.Empty);
            }
            else
            {
                StartCoroutine(RespawnDelayed());
            }

        }

        IEnumerator RespawnDelayed()
        {
            yield return new WaitForSeconds(RespawnTime);
            Debug.Log("Player died, spawning new one");
            SpawnPlayer();
        }

        private List<Destructable> mEnemies;

        void SpawnEnemy()
        {
            //return;

            Vector3 pos = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), 0);

            Transform EnemyPrefab = EnemyPrefabList[Random.Range(0, EnemyPrefabList.Length)];
            Transform t = (Transform)Instantiate(EnemyPrefab, pos, Quaternion.identity);
            Destructable e = (Destructable)t.gameObject.GetComponent<Destructable>();
            
            mEnemies.Add(e);

            Enemy en = (Enemy)t.gameObject.GetComponent<Enemy>();
            if (en != null)
                en.mainCanvas = MainCanvas;

            e.DeadEvent += new Destructable.Destroyed(EnemyDiedEvent);
        }

        void EnemyDiedEvent(System.EventArgs _args)
        {
            DamageEventArgs d = _args as DamageEventArgs;
            mEnemies.Remove(d.DamagedObject);

            AddScore(10);
        }

        public float GetRemainingTime()
        {
            return mSettings.LevelDuration - mGameDuration;
        }

        private float mSpawnTimer;

        IEnumerator GameTimer()
        {
            while (mGameDuration < mSettings.LevelDuration)
            {
                if (Time.timeScale != 0 && mSettings.EnemySpawnTime > 0) // If the game is not paused
                {
                    mGameDuration += Time.fixedDeltaTime;
                    mSpawnTimer += Time.fixedDeltaTime;

                    if (mSpawnTimer > mSettings.EnemySpawnTime) // Spawn a new enemy if the time is right
                    {
                        mSpawnTimer = 0;
                        SpawnEnemy();
                    }
                }
                else if (mEnemies.Count <= 0) // This case executes if we don't have spawned enemies
                {
                    bEnded = true;
                    if (PlayerWin != null)
                        PlayerWin(this, System.EventArgs.Empty);
                }

                
                yield return new WaitForFixedUpdate();
            }

            mGameDuration = mSettings.LevelDuration;
            bEnded = true;
            List<Destructable> enemyList = new List<Destructable>(mEnemies);
            

            foreach (Destructable m in enemyList)
            {
                m.Kill(null);
            }

            if (PlayerWin != null)
                PlayerWin(this, System.EventArgs.Empty);
        }


    }

}