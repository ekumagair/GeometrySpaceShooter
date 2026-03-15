package com.EduardoKumagai.GeometrySpaceShooter.agesignals;

import android.app.Activity;
import android.content.Context;

import com.google.android.play.agesignals.AgeSignalsManager;
import com.google.android.play.agesignals.AgeSignalsManagerFactory;
import com.google.android.play.agesignals.AgeSignalsRequest;
import com.google.android.play.agesignals.AgeSignalsResult;
import com.google.android.play.agesignals.model.AgeSignalsVerificationStatus;

import com.unity3d.player.UnityPlayer;

public class AgeSignalsPlugin {

    private static AgeSignalsManager manager;

    public static void CheckAgeSignals(final String unityGameObject) {
		
		UnityPlayer.UnitySendMessage(unityGameObject, "OnAgeSignalsDebug", "CheckAgeSignals [start]");

        Activity activity = UnityPlayer.currentActivity;
		
		if (activity == null) {
			return;
		}
		
		UnityPlayer.UnitySendMessage(unityGameObject, "OnAgeSignalsDebug", "Activity OK");
		
        Context context = activity.getApplicationContext();
		
		UnityPlayer.UnitySendMessage(unityGameObject, "OnAgeSignalsDebug", "Context OK");

        if (manager == null) {
            manager = AgeSignalsManagerFactory.create(context);
        }
		
		UnityPlayer.UnitySendMessage(unityGameObject, "OnAgeSignalsDebug", "Manager OK");

        AgeSignalsRequest request = AgeSignalsRequest.builder().build();
		
		UnityPlayer.UnitySendMessage(unityGameObject, "OnAgeSignalsDebug", "Request OK");

        manager
			.checkAgeSignals(request)
            .addOnSuccessListener(
				ageSignalsResult -> {

					String status = ageSignalsResult.userStatus().toString();

					Integer lowerObj = ageSignalsResult.ageLower();
					int lowerAge = lowerObj != null ? lowerObj : -1;

					Integer upperObj = ageSignalsResult.ageUpper();
					int upperAge = upperObj != null ? upperObj : -1;

					String message = status + "|" + lowerAge + "|" + upperAge;
					
					UnityPlayer.UnitySendMessage(
						unityGameObject,
						"OnAgeSignalsResult",
						message
					);
					
					UnityPlayer.UnitySendMessage(unityGameObject, "OnAgeSignalsDebug", "addOnSuccessListener");
				}
			)
			.addOnFailureListener(
				e -> {

					UnityPlayer.UnitySendMessage(
						unityGameObject,
						"OnAgeSignalsError",
						e.getMessage()
					);
					
					UnityPlayer.UnitySendMessage(unityGameObject, "OnAgeSignalsDebug", "addOnFailureListener");
				}
			);
		
		UnityPlayer.UnitySendMessage(unityGameObject, "OnAgeSignalsDebug", "CheckAgeSignals [end]");
    }
}