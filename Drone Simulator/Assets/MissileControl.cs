using UnityEngine;
using System.Collections;

public class MissileControl : MonoBehaviour {

    // missile properties
   
    public Rigidbody Missile;
    public Transform Launcher;
    public float missileVelocity = 300;
    public float turn = 20.0f;
    public float fuseDelay;
    public AudioClip missileAudio;
    public ParticleSystem smokePrefab;
    public GameObject missileModel;
    public Transform Target;
    public GameObject Explosion;

    // Use this for initialization
    void Start () {
        Missile = transform.GetComponent<Rigidbody>();
        Fire();
	}

    void Fire()
    {
        GetComponent<AudioSource>().Play();

        float distance = Mathf.Infinity;
        GameObject go = GameObject.FindGameObjectWithTag("target");

        float diff = (go.transform.position - transform.position).sqrMagnitude;

        if (diff < distance)
        {
            distance = diff;
            Target = go.transform;
        }
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (Target == null || Missile == null) return;
        Missile.velocity = transform.forward * missileVelocity;

        Quaternion targetRotation = Quaternion.LookRotation(Target.position - transform.position);

        Missile.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, turn));
	}

    IEnumerator OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "target")
        {
            Debug.Log("Collided!");
            Instantiate(Explosion, col.gameObject.transform.position, Quaternion.identity);
            smokePrefab.Stop();
            Destroy(missileModel.gameObject);
            yield return new WaitForSeconds(5);
            Destroy(gameObject);
        }

    }
}
