using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed;
	public Text countText;
	public Text winText;
    
    public AudioSource rollSource;
    public AudioSource impactSource;
    public AudioSource winSource;

    private Rigidbody rb;
    //private AudioSource audioSource;
    private Vector3 lastPosition;
    private bool foundObject = false;
    private float time;

    void Start() {
		rb = GetComponent<Rigidbody> ();
        //audioSource = GetComponent<AudioSource>();

        winText.text = string.Empty;
	}

	private string getTimer() {
        time += Time.deltaTime;

        var minutes = time / 60; //Divide the guiTime by sixty to get the minutes.
        var seconds = time % 60;//Use the euclidean division for the seconds.
        var fraction = (time * 100) % 100;

        //update the label value
        return string.Format("{0:0}m {1:0}s {2:00}ms", minutes, seconds, fraction);
    }

	void FixedUpdate() {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		var movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
        var velocity = System.Math.Abs((rb.velocity.x + rb.velocity.z));
        rollSource.volume = velocity * 0.05f;
        rollSource.pitch = (velocity * 0.03f) + 0.7f;

        rb.AddForce (movement * speed);
        if (!foundObject)
        {
            SetCountText();
        }
	}

	void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Pick Up")) {
			other.gameObject.SetActive (false);
            foundObject = true;
            SetCountText ();
		}
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject != null && collision.gameObject.CompareTag("Wall"))
        {
            impactSource.pitch = Random.Range(0.9F, 1.1F);
            impactSource.volume = collision.relativeVelocity.magnitude * 0.1f;
            impactSource.Play();
        }

    }

    private void SetCountText() {
		var timer = getTimer ();
		countText.text = string.Format("Timer: {0}", timer);
		if (foundObject) {
			winText.text = string.Format("You found it in {0}!", timer);
            winSource.Play();
        }
	}
}
