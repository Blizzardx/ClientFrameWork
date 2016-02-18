using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayMonsterAnimations : MonoBehaviour {

	public static PlayMonsterAnimations instance;
	AnimationClip mIdle;
	public ParticleSystem[] eatParticleArray;
	public Animator animator;
	AnimatorStateInfo currentBaseStage;
	int idle = Animator.StringToHash("Base Layer.daiji");
	int eat = Animator.StringToHash("Base Layer.chi");
	int eat2 = Animator.StringToHash("Base Layer.chi2");

	void Awake()
	{
		if(instance == null)
			instance = this;
	}

	void Start()
	{
		if(animator == null)
			animator = GetComponentInChildren<Animator>();
	}

	public void ShakeHead()
	{
		if(animator == null) return;
		animator.SetBool("ShakeHead",true);
		float time = 1f;
		CancelInvoke("PlayShakeToIdel");
		Invoke("PlayShakeToIdel",time);
	}

	public void PlayEatAnimation()
	{
		if(animator == null) return;

		currentBaseStage = animator.GetCurrentAnimatorStateInfo(0);
		float time = 1f;
		if(currentBaseStage.nameHash == idle)
		{
			animator.Play("chi");
			CancelInvoke("PlayEatEffect");
			Invoke("PlayEatEffect",time/2);
		}
		else if(currentBaseStage.nameHash == eat)
		{
			animator.Play("chi2");
			CancelInvoke("PlayEatEffect");
			Invoke("PlayEatEffect",time/2);
		}
		else if(currentBaseStage.nameHash == eat2)
		{
			animator.Play("chi");
			CancelInvoke("PlayEatEffect");
			Invoke("PlayEatEffect",time/2);
		}else{
			animator.Play("chi");
			CancelInvoke("PlayEatEffect");
			Invoke("PlayEatEffect",time/2);
		}
		CancelInvoke("PlayIdle");
		Invoke("PlayIdle",time);
	}

	public void PlayEatEffect()
	{
		foreach(var i in eatParticleArray)
		{
			i.Play();
		}
	}

	void PlayIdle()
	{
		animator.Play("daiji");
//		if(animator != null)
//			animator.SetBool("Eat",false);
	}

	void PlayShakeToIdel()
	{
		animator.SetBool("ShakeHead",false);
	}
}
