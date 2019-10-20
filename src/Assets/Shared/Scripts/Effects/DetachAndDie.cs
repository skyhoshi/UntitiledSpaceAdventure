using UnityEngine;
using System.Collections;

public class DetachAndDie : MonoBehaviour {

    public void RunDetachAndDie()
    {
        transform.parent = null;
        ParticleSystem p = gameObject.GetComponent<ParticleSystem>();
        //p.emissionRate = 0;
        var em = p.emission;
        em.enabled = false;
        p.loop = false;

        Destroy(gameObject, p.duration);
    }
}
