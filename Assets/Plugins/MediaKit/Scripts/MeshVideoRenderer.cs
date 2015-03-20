using UnityEngine;

public class MeshVideoRenderer : MonoBehaviour {

	public VideoPlayer VideoPlayer;
	
	MeshFilter meshFilter_ = null;

	Renderer renderer_ = null;

	void Start()
	{
		renderer_ = GetComponent<Renderer>();
		meshFilter_ = GetComponent<MeshFilter>();
	}

	void Update()
	{

		if(VideoPlayer != null)
		{
			if(VideoPlayer.IsPlaying){

				if(renderer_ != null)
					renderer_.enabled = false;
			
				Graphics.DrawMesh(meshFilter_.sharedMesh,transform.localToWorldMatrix,VideoPlayer.VideoOutput,0);
			}
			else
			{
				if(renderer_ != null)
					renderer_.enabled = true;
			}

		}
	}
}
