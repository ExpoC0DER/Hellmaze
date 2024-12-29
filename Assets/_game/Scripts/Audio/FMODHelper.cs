using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace _game.Scripts
{
	public static class FMODHelper
	{
		public static bool InstanceIsPlaying(EventInstance instance)
		{
			instance.getPlaybackState(out PLAYBACK_STATE state);
			return state != PLAYBACK_STATE.STOPPED;
		}

		public static EventInstance PlayNewInstance(EventReference eventReference, Vector3? position3D = null)
		{
			EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
			if (position3D != null)
				eventInstance.set3DAttributes(((Vector3)position3D).To3DAttributes());
			eventInstance.start();
			eventInstance.release();
			return eventInstance;
		}

		public static EventInstance PlayNewInstance(EventReference eventReference, Transform objectToAttachTo)
		{
			EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
			if (objectToAttachTo)
				RuntimeManager.AttachInstanceToGameObject(eventInstance, objectToAttachTo);
			eventInstance.start();
			eventInstance.release();
			return eventInstance;
		}

		public static EventInstance CreateNewInstance(EventReference eventReference, Vector3? position3D = null)
		{
			EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
			if (position3D != null)
				eventInstance.set3DAttributes(((Vector3)position3D).To3DAttributes());
			return eventInstance;
		}
		
		public static EventInstance CreateNewInstance(EventReference eventReference, Transform objectToAttachTo)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            if (objectToAttachTo)
				RuntimeManager.AttachInstanceToGameObject(eventInstance, objectToAttachTo);
            return eventInstance;
        }
	}
}
