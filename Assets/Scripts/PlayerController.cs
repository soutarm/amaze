using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public float speed;
	public Text countText;
	public Text winText;
    public Text camText;
    
    public AudioSource rollSource;
    public AudioSource impactSource;
    public AudioSource winSource;

    public Camera cam1;
    public Camera cam2;

    private Rigidbody rb;
    private int frameWait = 0, camTimeout = 0;
    private Vector3 lastPosition;
    private bool foundObject = false;
    private float time;

    void Start() {
		rb = GetComponent<Rigidbody> ();
        //audioSource = GetComponent<AudioSource>();

        winText.text = string.Empty;
        cam1.enabled = true;
        cam2.enabled = false;
        SetCamText();
    }

	private string getTimer() {
        time += Time.deltaTime;

        var minutes = time / 60; //Divide the guiTime by sixty to get the minutes.
        var seconds = time % 60;//Use the euclidean division for the seconds.
        var fraction = (time * 100) % 100;

        //update the label value
        return string.Format("{0:0}:{1:00}:{2:00}", minutes, seconds, fraction);
    }

    private string getTotalSeconds()
    {
        time += Time.deltaTime;
        return string.Format("{0:0}", time % 60);
    }

    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.R) && foundObject)
        if (Input.GetKeyUp(KeyCode.R))
        {
            // reset the maze
            SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        }

        if (Input.GetKey(KeyCode.C) && camTimeout > 10)
        {
            // toggle camera
            cam1.enabled = !cam1.enabled;
            cam2.enabled = !cam2.enabled;
            camTimeout = 0;

            SetCamText();
        }
        // Prevents toggling camera repeatedly
        camTimeout = camTimeout + 1;
    }

    private void updateBallSound(Vector3 movement)
    {
        var velocity = System.Math.Abs((rb.velocity.x + rb.velocity.z));
        rollSource.volume = velocity * 0.05f;
        rollSource.pitch = (velocity * 0.03f) + 0.7f;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");


        var movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        updateBallSound(movement);

        rb.AddForce(movement * speed);
        if (!foundObject)
        {
            SetCountText();
        }

        // Make win text go crazy
        if (foundObject && frameWait > 4)
        {
            winText.color = new Color(Random.Range(0F, 1F), Random.Range(0F, 1F), Random.Range(0F, 1F));
            frameWait = 0;
        } else
        {
            frameWait = frameWait + 1;
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

    private void SetCamText()
    {
        camText.text = cam1.enabled ? "Chase Cam" : "Bird's Eye";
    }

    private void SetCountText() {
		var timer = getTimer ();
		countText.text = string.Format("TIME {0}", timer);
		if (foundObject) {
			winText.text = string.Format("You found it in\n{0} seconds!\nPress 'r' to restart", getTotalSeconds());
            winSource.Play();
        }
	}

}
