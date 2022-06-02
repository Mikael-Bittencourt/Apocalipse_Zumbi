using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour {

	public float Velocidade = 20;
	private Rigidbody rigidbodyBala;
	public AudioClip SomDeMorte;

	void Start ()
	{
		rigidbodyBala = GetComponent<Rigidbody> ();
	}
	// Update is called once per frame
	void FixedUpdate () {
		rigidbodyBala.MovePosition
		(rigidbodyBala.position + 
			transform.forward * Velocidade * Time.deltaTime);
	}

	void OnTriggerEnter(Collider objetoDeColisao)
	{
		Quaternion rotacaoOpostaABala = Quaternion.LookRotation(-transform.forward);
		switch(objetoDeColisao.tag) 
		{
		case"Inimigo":
			Controla_Inimigo inimigo = objetoDeColisao.GetComponent<Controla_Inimigo>();
			inimigo.TomarDano(1);
			inimigo.ParticulaSangue(transform.position, rotacaoOpostaABala);
			break;
		case "ChefeDeFase":
			ControlaChefe chefe = objetoDeColisao.GetComponent<ControlaChefe>();
			chefe.TomarDano(1);
			chefe.ParticulaSangue(transform.position, rotacaoOpostaABala);
		break;
		}

		Destroy(gameObject);
	}
}
