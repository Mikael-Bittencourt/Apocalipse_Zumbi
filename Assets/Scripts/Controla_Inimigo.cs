using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controla_Inimigo : MonoBehaviour, IMatavel
{

	public GameObject Jogador;
	private MovementoPersonagen MovimentaInimigo;
	private AnimacaoPersonagem animacaoInimigo;
	private Status statusInimigo;
	public AudioClip SomDeMorte;
	private Vector3 posicaoAleatoria;
	private Vector3 direcao;
	private float contadorVaga;
	private float tempoEntrePosicoesAleatorias = 4;
	private float porcentagemGerarKitMedico = 0.1f;
	public GameObject KitMedicoPrefab;
	private ControlaInterface scriptControlaInterface;
	[HideInInspector]
	public GeradorZumbis meuGerador;
	public GameObject ParticulaSangueZumbi;

	// Use this for initialization
	void Start () {
		Jogador = GameObject.FindWithTag("Jogador");
		animacaoInimigo = GetComponent<AnimacaoPersonagem>();
		MovimentaInimigo = GetComponent<MovementoPersonagen>();
		AleatorizarZumbis();
		statusInimigo = GetComponent<Status>();
		scriptControlaInterface = GameObject.FindObjectOfType(typeof(ControlaInterface)) as ControlaInterface;
	}

	void FixedUpdate() {

		float distancia = Vector3.Distance (transform.position,Jogador.transform.position);

		MovimentaInimigo.Rotacionar(direcao);

		animacaoInimigo.Movimentar (direcao.magnitude);

		if (distancia > 15) 
		{
			Vagar();
		}
		else if (distancia > 2.5) {
			direcao = Jogador.transform.position - transform.position;

			MovimentaInimigo.Movimentar(direcao, statusInimigo.Velocidade);

			animacaoInimigo.Atacar(false);
		} else 
		{
			direcao = Jogador.transform.position - transform.position;

			animacaoInimigo.Atacar(true);
		}
	}
	void Vagar ()
	{
		contadorVaga -= Time.deltaTime;
		if(contadorVaga <= 0)
		{
			posicaoAleatoria = AleatorizarPosicao();
			contadorVaga += tempoEntrePosicoesAleatorias + Random.Range(-1f, 1f);
		}

		bool ficouPertoOSuficiente = Vector3.Distance (transform.position, posicaoAleatoria) <= 0.05;
		if(ficouPertoOSuficiente == false)
		{
			direcao = posicaoAleatoria - transform.position;
			MovimentaInimigo.Movimentar(direcao, statusInimigo.Velocidade);
		}
	}

	Vector3 AleatorizarPosicao ()
	{
		Vector3 posicao = Random.insideUnitSphere * 10;
		posicao += transform.position;
		posicao.y = transform.position.y;

		return posicao;
	}

	void AtacaJogador ()
	{
		int dano = Random.Range (20, 30);
		Jogador.GetComponent<ControlaJogador> ().TomarDano(dano);
	}

	void AleatorizarZumbis ()
	{
		int GeraTipoZumbi = Random.Range(1, transform.childCount);
		transform.GetChild (GeraTipoZumbi).gameObject.SetActive (true);
	}

	public void TomarDano(int dano)
	{
		statusInimigo.Vida -= dano;
		if(statusInimigo.Vida <= 0)
		{
			Morrer();
		}
	}

	public void ParticulaSangue (Vector3 posicao, Quaternion rotacao)
	{
		Instantiate(ParticulaSangueZumbi, posicao, rotacao);
	}

	public 	void Morrer()
	{
		Destroy(gameObject, 2);
		animacaoInimigo.Morrer();
		MovimentaInimigo.Morrer();
		this.enabled = false;
		ControlaAudio.instancia.PlayOneShot(SomDeMorte);
		VerificarGeracaoKitMedico (porcentagemGerarKitMedico);
		scriptControlaInterface.AtualizarQuantidadeDeZumbisMortos();
		meuGerador.DiminuirQuantidadeDeZumbisVivos();
	}

	void VerificarGeracaoKitMedico(float porcentagemGeracao)
	{
		if (Random.value <= porcentagemGeracao) 
		{
			Instantiate (KitMedicoPrefab, transform.position, Quaternion.identity);
		}
	}
}
