using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace ChristmasBattle
{
	public static class MediaManager
	{		

		public enum Sound
		{
			Rudolf_A1,
			Snowman_A1,
			Santa_A1,
			Elf_A1,
			Enemy_damage,
			ButtonPress
		}

		public enum Effect
		{
			BasicDamage,
			Shot,
			RudolfBigPunch,
			Magic,
			Corrupted,
			Heal,
			Energy
		}

		public static void PlaySound(Sound sound)
		{
			GameObject soundObject = new GameObject("Sound");
			AudioSource audioSource = soundObject.AddComponent<AudioSource>();
			audioSource.clip = GetAudioClip(sound);
			audioSource.PlayOneShot(audioSource.clip);
			Object.Destroy(soundObject, audioSource.clip.length);
		}

		static AudioClip GetAudioClip(Sound sound)
		{
			foreach (var clip in DataHolder.S.SoundAudioClips)
			{
				if (clip.sound == sound)
					return clip.audioClip;
			}
			Debug.LogError("Sound " + sound + " not found!");
			return null;
		}

		public static void PlayEffect(Effect effect, Vector2 position)
		{
			GameObject effectObject = Object.Instantiate(GetEffect(effect));
			effectObject.transform.position = position;
			var PS = effectObject.transform.gameObject.GetComponent<ParticleSystem>();
			if (!PS.loop)
				Object.Destroy(effectObject, PS.main.duration + PS.main.startLifetime.constant);

		}

		static GameObject GetEffect(Effect effect)
		{
			foreach (var e in DataHolder.S.Effects)
			{
				if (e.effect == effect)
					return e.EffectObject;
			}
			Debug.LogError("Sound " + effect + " not found!");
			return null;
		}
	}
}