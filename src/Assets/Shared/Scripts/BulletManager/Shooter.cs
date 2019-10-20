using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Config for setting which port to use on a ship
/// TODO: Set simultaneous as a tick box
/// </summary>
public enum FirePortPreference
{
    AllPorts,
    PrimaryPort,
    AllPortsSimultaneous,
    Even,
    Odd,
}

/// <summary>
/// Sets the type of shot. Single fires a single bullet.
/// Burst fires multiple bullets with a configurable delay between them (can be 0)
/// </summary>
public enum ShotType
{
    Single,
    Burst,
    //Beam, // Feature comming soon.
    //Charge, 
}

/// <summary>
/// Data for a weapon used by shooter. This contains the firing information for a type of weapon, which consists of a bullet muzzle flashes and cooldowns.
/// </summary>
[System.Serializable]
public class Weapon
{
    /// <summary>
    /// Weapon name (taken from the prefab name) Must be unique.
    /// </summary>
    [UnityEngine.SerializeField]
    public string Name;

    /// <summary>
    /// The bullet to fire (can be mortar etc)
    /// </summary>
    [UnityEngine.SerializeField]
    public Transform BulletPrefab;

    /// <summary>
    /// (optional) Muzzle flash when a bullet is fired
    /// </summary>
    [UnityEngine.SerializeField]
    public Transform MuzzleFlashPrefab;

    /// <summary>
    /// Time between shots
    /// </summary>
    [UnityEngine.SerializeField]
    public float Cooldown;

    /// <summary>
    /// Which port to fire the bullet from
    /// </summary>
    [UnityEngine.SerializeField]
    public FirePortPreference PortPreference;

    /// <summary>
    /// Type of shot
    /// </summary>
    [UnityEngine.SerializeField]
    public ShotType ShotType;

    /// <summary>
    /// Options for burst shots. Total number of shots in the sequence
    /// </summary>
    [Tooltip("Number of shots in the sequence")]
    public int ShotNo       = 2;

    /// <summary>
    /// Options for burst shots. Time between shots
    /// </summary>
    [Tooltip("Time between shots")]
    public float ShotDelay  = 0.03f;

}

/// <summary>
/// AttackData that is passed with the Fire() function to all bullets that are created.
/// Extend this class to add your own custom data for powerups, bonuses, buffs, modifiers or stat collection data
/// </summary>
public class AttackData
{
    /// <summary>
    /// List of targets passed by firer. May be empty
    /// </summary>
    private List<Transform> TargetList = new List<Transform>();

    /// <summary>
    /// Add a target to the target list
    /// </summary>
    /// <param name="_target">Target transform</param>
    public void AddTarget(Transform _target)
    {
        if (TargetList.Contains(_target) == false)
            TargetList.Add(_target);
    }

    /// <summary>
    /// Returns a (new) list of targets
    /// </summary>
    /// <returns>copy of TargetList</returns>
    public List<Transform> GetTargets()
    {
        return new List<Transform>(TargetList);
    }

}

/// <summary>
/// Class that handles firing bullets and weapons. Manages fire ports, cooldowns, fire rates passing of data and setting colliders
/// Can be used with a Transform, a bullet or a weapon.
/// </summary>
public class Shooter : MonoBehaviour {

    /// <summary>
    /// Requires a rigidbody
    /// </summary>
    private Rigidbody2D rb2d;

    /// <summary>
    /// Cached colliders for disabling collisions
    /// </summary>
    private Collider2D[] mColliders;

    /// <summary>
    /// Dic containing cooldowns per weapon
    /// </summary>
    private Dictionary<string, float> mDicCooldowns = new Dictionary<string, float>();

    /// <summary>
    /// Tracks the current port rotation
    /// </summary>
    private int mPortCount = 0;

    /// <summary>
    /// List of points that this object fires from, this is optional
    /// </summary>
    [Tooltip("List of points that this object fires from, this is optional")]
    public Transform[] _FirePorts;
    public Transform[] FirePorts
    {
        get => _FirePorts;
        set => _FirePorts = value;
    }

    /// <summary>
    /// Cache of the last fired bullet
    /// </summary>
    private Bullet LastBullet;

    /// <summary>
    /// Gets the last bullet fired by this system
    /// </summary>
    /// <returns>last Bullet</returns>
    public Bullet GetLastBullet()
    {
        return LastBullet;
    }

    /// <summary>
    /// Returns the number of ports, sets ports
    /// </summary>
    public int PortCount
    {
        get { return mPortCount; }
        set
        {
            if (value >= 0 && value < FirePorts.Length)
                mPortCount = value;
            else
                Debug.LogError(string.Format("Set ports on Shooter {0} failed. Value: {1}, there are only {2} ports", name, value.ToString(), FirePorts.Length.ToString()) );
        }
    }
    
    /// <summary>
    /// Allows setting of new fireports. 
    /// </summary>
    /// <param name="_newPorts"></param>
    public void SetFirePorts(Transform[] _newPorts)
    {
        mPortCount = 0;
        FirePorts = _newPorts;
    }

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
            rb2d = gameObject.GetComponentInParent<Rigidbody2D>();

        if (FirePorts == null || FirePorts.Length == 0) // Set the fire port to this object if none are provided
        {
            Debug.Log("Created new ports: " + name);
            FirePorts = new Transform[1];
            FirePorts[0] = transform;
        } 

        UpdateColliders();
    }

    /// <summary>
    /// Updates the colliders on this object
    /// </summary>
    public void UpdateColliders()
    {
        if (rb2d != null)
            mColliders = rb2d.gameObject.GetComponentsInChildren<Collider2D>();
        else
            mColliders = GetComponentsInChildren<Collider2D>();
    }

    /// <summary>
    /// Sets ignore collisions between all the colliders in the bullet prefab and the colliders in this game object hierachy (defined by the rigidbody)
    /// </summary>
    /// <param name="_bullet">Object to ignore collisions with</param>
    private void IgnoreOwnColliders(Transform _bullet)
    {
        IgnoreColliders(_bullet, mColliders);
    }

    /// <summary>
    /// Allows prevention of collision between a specific bullet and a set of colliders
    /// </summary>
    /// <param name="_bullet">Bullet to ignore collision with</param>
    /// <param name="_colliders">Colliders of object we want to ignore collisions with</param>
    public static void IgnoreColliders(Transform _bullet, Collider2D[] _colliders)
    {
        Collider2D[] colliders = _bullet.gameObject.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D BulletCol in colliders)
        {
            foreach (Collider2D selfCol in _colliders)
                Physics2D.IgnoreCollision(BulletCol, selfCol);
        }

    }

    /// <summary>
    /// Allows toggling of collisions between objects
    /// </summary>
    /// <param name="_bullet">Bullet we want to ignore collision with</param>
    /// <param name="_colliders">Colliders of object</param>
    /// <param name="_ignore">Bool ignore (assuming function does nothing if this is set to false)</param>
    public static void IgnoreColliders(Transform _bullet, Collider2D[] _colliders, bool _ignore)
    {

        Collider2D[] colliders = _bullet.gameObject.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D BulletCol in colliders)
        {
            foreach (Collider2D selfCol in _colliders)
            {
                Physics2D.IgnoreCollision(BulletCol, selfCol, _ignore);

            }
        }
    }

    /// <summary>
    /// Most basic fire function, takes a prefab to spawn
    /// </summary>
    /// <param name="_prefab">Prefab to spawn</param>
    /// <returns>Returns true if a bullet was created</returns>
    public bool Fire(Transform _prefab)
    {
        return Fire(_prefab, null);
    }

    /// <summary>
    /// Most basic fire function, takes a prefab to spawn and passes on attack data
    /// </summary>
    /// <param name="_prefab">Prefab to spawn</param>
    /// /// <param name="_data">Data to pass on</param>
    /// <returns>Returns true if a bullet was created</returns>
    public bool Fire(Transform _prefab, AttackData _data)
    {
        //Debug.Log("PortCount " + mPortCount);

        WeaponConfig wc = _prefab.GetComponent<WeaponConfig>();
        
        if (wc != null)
        {
            
            return Fire(wc, _data);
        }
        Bullet b = SpawnProjectile(_prefab, FirePorts[mPortCount].position, FirePorts[mPortCount].rotation, _data);
        
        mPortCount++;

        if (mPortCount >= FirePorts.Length)
            mPortCount = 0;

        return true;
    }

    /// <summary>
    /// Advanced fire function, takes a weapon config which has shot configurations
    /// </summary>
    /// <param name="_weaponConfig">Weapon Config</param>
    /// <returns>Returns true if a bullet was created</returns>
    public bool Fire(WeaponConfig _weaponConfig)
    {
        return Fire(_weaponConfig.GetWeapon());
    }

    /// <summary>
    /// Advanced fire function, takes a weapon config which has shot configurations. Passes on attack data to bullets
    /// </summary>
    /// <param name="_weaponConfig">Weapon Config</param>
    /// <param name="_data">Data to pass on</param>
    /// <returns>Returns true if a bullet was created</returns>
    public bool Fire(WeaponConfig _weaponConfig, AttackData _data)
    {
        return Fire(_weaponConfig.GetWeapon(), _data);
    }

    /// <summary>
    /// Advanced fire function, takes a weapon which has shot configurations.
    /// </summary>
    /// <param name="_weaponConfig">Weapon (stripped out from weapon config)</param>
    /// <returns>Returns true if a bullet was created</returns>
    public bool Fire(Weapon _weapon)
    {
        return Fire(_weapon, null);
    }

    /// <summary>
    /// Advanced fire function, takes a weapon which has shot configurations. Passes on attack data to bullets
    /// </summary>
    /// <param name="_weaponConfig">Weapon (stripped out from weapon config)</param>
    /// <param name="_data">Data to pass on</param>
    /// <returns>Returns true if a bullet was created</returns>
    public bool Fire(Weapon _weapon, AttackData _data)
    {
        Bullet b = null;

        if (mDicCooldowns.ContainsKey(_weapon.Name) == false) // Check for cooldown entry
            mDicCooldowns.Add(_weapon.Name, Time.realtimeSinceStartup);

        if (Time.realtimeSinceStartup - mDicCooldowns[_weapon.Name] < _weapon.Cooldown)
            return false; // Check weapon has coolded down.

        mDicCooldowns[_weapon.Name] = Time.realtimeSinceStartup;

        if (_weapon.ShotType == ShotType.Single) // Fire specific shot type
        {
            b = FireSingleShot(_weapon, _data);
        }
        else
        {
            b = FireBurstShots(_weapon, _data);
        }
        return true;
    }

    /// <summary>
    /// Fires several shots in quick succession
    /// </summary>
    /// <param name="_weapon">Weapon being fired</param>
    /// <param name="_data">Passed attack data</param>
    /// <returns>Bullet that was created</returns>
    private Bullet FireBurstShots(Weapon _weapon, AttackData _data)
    {
        Bullet b = null;
        float t = 0;

        if (_weapon.PortPreference == FirePortPreference.PrimaryPort)
        {
            for (int i = 0; i < _weapon.ShotNo; i++)
            {
                StartCoroutine(FireFromPortDelayed(_weapon,0, t, _data));
                t += _weapon.ShotDelay;
            }
            
        }
        else if (_weapon.PortPreference == FirePortPreference.AllPorts)
        {
            for (int i = 0; i < _weapon.ShotNo; i++)
            {
                StartCoroutine(FireFromPortDelayed(_weapon, mPortCount, t, _data));
                t += _weapon.ShotDelay;
            }

            mPortCount++;

            if (mPortCount >= FirePorts.Length)
                mPortCount = 0;
        }
        else if (_weapon.PortPreference == FirePortPreference.Even)
        {
            for (int i = 0; i < _weapon.ShotNo; i++)
            {
                if (mPortCount >= FirePorts.Length)
                    mPortCount = 0;

                StartCoroutine(FireFromPortDelayed(_weapon, mPortCount, t, _data));

                mPortCount += 2;

                t += _weapon.ShotDelay;
            }
        }
        else if (_weapon.PortPreference == FirePortPreference.Odd)
        {
            for (int i = 0; i < _weapon.ShotNo; i++)
            {

                if (mPortCount >= FirePorts.Length || mPortCount == 0)
                    mPortCount = 1;

                for (int j = 1; j < FirePorts.Length; j += 2)
                    StartCoroutine(FireFromPortDelayed(_weapon, j, t, _data));

                mPortCount += 2;

                t += _weapon.ShotDelay;
            }
        }
        else if (_weapon.PortPreference == FirePortPreference.AllPortsSimultaneous)
        {
            for (int i = 0; i < _weapon.ShotNo; i++)
            {
                for (int j = 0; j < FirePorts.Length; j++)
                    StartCoroutine(FireFromPortDelayed(_weapon, j, t, _data));

                t += _weapon.ShotDelay;
            }
        }


       return b;
    }

    /// <summary>
    /// Handles firing a shot that is delayed
    /// </summary>
    /// <param name="_weapon">Firing weapon</param>
    /// <param name="_port">Port to use</param>
    /// <param name="_delay">Delay in seconds</param>
    /// <param name="_data">Data to pass on</param>
    /// <returns></returns>
    IEnumerator FireFromPortDelayed(Weapon _weapon, int _port, float _delay, AttackData _data)
    {
        if (_weapon == null)
            throw new System.Exception("Weapon is null");

        FireFromPortCache cache;
        cache.Weapon = _weapon;

        if (_port > FirePorts.Length)
            _port = 0;

        cache.Port = FirePorts[_port];

        cache.Data = _data;

        yield return new WaitForSeconds(_delay);

        FireFromPort(cache);
    }

    /// <summary>
    /// This function is unused
    /// This is not well defined. In the current implimentation it uses default ports, which includes simultneous fire. 
    /// </summary>
    /// <param name="_weapon"></param>
    /// <param name="_delay"></param>
    /// <param name="_data"></param>
    /// <returns></returns>
    IEnumerator FireSingleShotDelayed(Weapon _weapon, float _delay, AttackData _data)
    { // TODO: Make this fit with how delayed multi fire is done (FireFromPortDelayed)
        yield return new WaitForSeconds(_delay);

        FireSingleShot(_weapon, _data);   
    }

    /// <summary>
    /// Launches a single shot - selects a port based on port preferences
    /// </summary>
    /// <param name="_weapon">Weapon to fire</param>
    /// <param name="_data">Data to pass to the bullet</param>
    /// <returns>Returns created bullet</returns>
    private Bullet FireSingleShot(Weapon _weapon, AttackData _data)
    {
        return FireSingleShot(_weapon, -1, _data);
    }

    /// <summary>
    /// Launches a single shot from specified port, port of -1 uses the port preferences of the current weapon
    /// </summary>
    /// <param name="_weapon">Weapon to fire</param>
    /// <param name="_port">Port to fire from</param>
    /// <param name="_data">Data to pass to bullet</param>
    /// <returns>Created bullet</returns>
    private Bullet FireSingleShot(Weapon _weapon, int _port, AttackData _data)
    {
        Bullet b = null;

        int chosenPort = 0; 
        if (_port > 0 && _port < FirePorts.Length) // Check port is valid
            chosenPort = _port;

        // Updates port based on port preferences
        if (_weapon.PortPreference == FirePortPreference.PrimaryPort)
            chosenPort = 0;
        else if (_weapon.PortPreference == FirePortPreference.AllPorts)
        {
            chosenPort = mPortCount;

            mPortCount++;

            if (mPortCount >= FirePorts.Length)
                mPortCount = 0;
        }
        else if (_weapon.PortPreference == FirePortPreference.Even)
        {

            chosenPort = mPortCount;

            mPortCount += 2;
            if (mPortCount >= FirePorts.Length)
                mPortCount = 0;
        }
        else if (_weapon.PortPreference == FirePortPreference.Odd)
        {
            if (mPortCount >= FirePorts.Length || mPortCount == 0)
                mPortCount = 1;

            chosenPort = mPortCount;

            mPortCount += 2;

        }
        else if (_weapon.PortPreference == FirePortPreference.AllPortsSimultaneous)
        { // Note that simultaneous is different because it produces multiple bullets instead of a single one.
            for (int i = 0; i < FirePorts.Length; i++)
                b = FireFromPort(_weapon, i, _data);

            return b;
        }

        b = FireFromPort(_weapon, chosenPort, _data);
        return b;
    }

    /// <summary>
    /// Spawns muzzle flash and triggers spawning the bullet prefab from selected port
    /// </summary>
    /// <param name="_weapon">Firing weapon</param>
    /// <param name="PortNo">Selected port</param>
    /// <param name="_data">Data to pass to the bullet</param>
    /// <returns>Bullet created</returns>
    private Bullet FireFromPort(Weapon _weapon, int PortNo, AttackData _data)
    {
        if (PortNo > FirePorts.Length) // validate port
            PortNo = 0; 

        if (_weapon == null) // validate weapon
            throw new System.Exception("Weapon is null");

        Bullet b = SpawnProjectile(_weapon.BulletPrefab, FirePorts[PortNo].position, FirePorts[PortNo].rotation, _data);

        if (_weapon.MuzzleFlashPrefab != null)
            SpawnMuzzleFlash(_weapon.MuzzleFlashPrefab, FirePorts[PortNo].position, FirePorts[PortNo].rotation);

        return b;        
    }

    /// <summary>
    /// This is a cached version of FireFromPort, for shots that are delayed. We need to remember the selected port at the time of firing rather than
    /// use the current port as the port number may have increased since we issued the command
    /// </summary>
    /// <param name="_cache">Cached data</param>
    /// <returns>Fired  bullet</returns>
    private Bullet FireFromPort(FireFromPortCache _cache)
    {
        Bullet b = SpawnProjectile(_cache.Weapon.BulletPrefab, _cache.Port.position, _cache.Port.rotation, _cache.Data);

        if (_cache.Weapon.MuzzleFlashPrefab != null)
            SpawnMuzzleFlash(_cache.Weapon.MuzzleFlashPrefab, _cache.Port.position, _cache.Port.rotation);

        return b;
    }

    /// <summary>
    /// Cached fire from port data
    /// </summary>
    public struct FireFromPortCache
    {
        public Weapon Weapon;
        public Transform Port;
        public AttackData Data;
    }

    /// <summary>
    /// List of all the bullets fired by this shooter
    /// </summary>
    [SerializeField]
    private List<Bullet> Bullets;

    /// <summary>
    /// Number of fire shots from this shooter
    /// </summary>
    private int NumFired = 0;

    /// <summary>
    /// Star of the show. Spawns the projectile, sets the layer, adds the bullet to bullet lists.
    /// Ignores shooters colliders. 
    /// </summary>
    /// <param name="_prefab"></param>
    /// <param name="_pos"></param>
    /// <param name="_rotation"></param>
    /// <param name="_data"></param>
    /// <returns></returns>
    protected Bullet SpawnProjectile(Transform _prefab, Vector3 _pos, Quaternion _rotation, AttackData _data)
    {
        Transform t = (Transform)Instantiate(_prefab, _pos, _rotation);
        Rigidbody2D rb = t.GetComponent<Rigidbody2D>();

        if (rb != null && rb2d != null)
            rb.velocity = rb2d.velocity;

        // Debug stuff
        NumFired++;

        IgnoreOwnColliders(t); // Ignore collisions with self. (In future add option to override this so bullets can collide with the shooter)

        Bullet b =  t.gameObject.GetComponent<Bullet>();

        if (b == null)
            return null;


        // TODO: Still requires testing
        // If the user specified a specific layer for this bullet, spawn it in that layer, else spawn it in the layer of the shooter.
        if (b.BulletCollisionSetting == BulletCollisionSettings.ParentLayer)
            t.gameObject.layer = gameObject.layer;

        else if (b.BulletCollisionSetting == BulletCollisionSettings.UserDefinedLayer)
            t.gameObject.layer = b.SpawnLayer.LayerIndex;

        else if (b.BulletCollisionSetting == BulletCollisionSettings.NoCollisions)
            t.gameObject.layer = LayerMask.NameToLayer("NoCollisions");

        else if (b.BulletCollisionSetting == BulletCollisionSettings.TargetListOnly)
        {
            Debug.Log("Setting collisions to target list only");
            t.gameObject.layer = LayerMask.NameToLayer("NoCollisions");
            List<Transform> targets = _data.GetTargets();

            foreach (Transform target in targets)
            {
                Debug.Log("Ignoring colliders on " + target.gameObject.name);
                IgnoreColliders(b.transform, target.GetComponents<Collider2D>(), false);
            }
        }
        else
            t.gameObject.layer = gameObject.layer;

        

        // Register for when this object is destroyed
        b.BulletDestroyedEvent += new Bullet.BulletDestroyed(HandleBulletDestroyedEvent);
        b.Owner = this.transform;
        b.AttackData = _data; // This is most likely null

        LastBullet = b; // Remember we made this
        Bullets.Add(b);

        return b;
    }

    /// <summary>
    /// Remove _b from the list as it is now null
    /// </summary>
    /// <param name="_b">Bullet b</param>
    void HandleBulletDestroyedEvent(Bullet _b)
    {
        if (Bullets.Contains(_b) == true)
            Bullets.Remove(_b);
    }

    /// <summary>
    /// Adds bullet to the current list of bullets
    /// Currently unused
    /// </summary>
    /// <param name="_b"></param>
    private void BulletListAdd(Bullet _b)
    {
        foreach (Bullet b in Bullets)
        {
            if (b == null)
                Bullets.Remove(b);
        }
    }

    /// <summary>
    /// Spawns the muzzle flash effect
    /// </summary>
    /// <param name="_prefab">Prefab to spawn</param>
    /// <param name="_pos">Location to spawn prefab</param>
    /// <param name="_rotation">rotation to spawn prefab</param>
    private void SpawnMuzzleFlash(Transform _prefab, Vector3 _pos, Quaternion _rotation)
    {
        Transform t = (Transform)Instantiate(_prefab, _pos, _rotation);
    }
}
