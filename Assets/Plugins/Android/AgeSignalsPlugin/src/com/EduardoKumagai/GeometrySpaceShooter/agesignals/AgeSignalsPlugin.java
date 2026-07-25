package com.EduardoKumagai.GeometrySpaceShooter.agesignals;

import android.app.Activity;
import android.content.Context;

import java.util.Date;

import com.google.android.play.agesignals.AgeSignalsManager;
import com.google.android.play.agesignals.AgeSignalsManagerFactory;
import com.google.android.play.agesignals.AgeSignalsAccessRequest;
import com.google.android.play.agesignals.AgeSignalsRequest;
import com.google.android.play.agesignals.AgeSignalsResult;
import com.google.android.play.agesignals.model.AgeSignalsStatus;

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

        AgeSignalsAccessRequest accessRequest = AgeSignalsAccessRequest.builder().setActivity(activity).build();
		
		UnityPlayer.UnitySendMessage(unityGameObject, "OnAgeSignalsDebug", "Request OK");
		
		manager.requestAgeSignalsAccess(accessRequest)
			.addOnSuccessListener(accessResult -> {
				
				Integer status = accessResult.ageSignalsStatus();
				
				if (status == AgeSignalsStatus.SHARED) {
					// The user (or parent) has agreed to share age range, or is in an eligible auto-share region.
					// Retrieve the actual age signals.
					retrieveAgeSignals(manager, unityGameObject);
				} else {
					// Age signals are not shared (user didn't share age range, parent rejected the request, or not eligible).
					resultInvalid(unityGameObject, "NOTSHARED");
				}
			})
			.addOnFailureListener(exception -> {
				
				String message = exception.getMessage();
				if (message == null) {
					message = exception.toString();
				}
				
				resultInvalid(unityGameObject, message);
			});

		UnityPlayer.UnitySendMessage(unityGameObject, "OnAgeSignalsDebug", "CheckAgeSignals [end]");
    }
	
	private static void retrieveAgeSignals(AgeSignalsManager manager, String unityGameObject) {
		
		manager.checkAgeSignals(AgeSignalsRequest.builder().build())
			.addOnSuccessListener(ageSignalsResult -> {
				
				String installId = ageSignalsResult.installId();

				Integer lowerObj = ageSignalsResult.ageLower();
				int lowerAge = lowerObj != null ? lowerObj : -1;

				Integer upperObj = ageSignalsResult.ageUpper();
				int upperAge = upperObj != null ? upperObj : -1;
				
				Date significantChangeDate = ageSignalsResult.significantChangeApprovalDate();

				String message = installId + "|" + lowerAge + "|" + upperAge;
					
				UnityPlayer.UnitySendMessage(
					unityGameObject,
					"OnAgeSignalsResult",
					message
				);
					
				UnityPlayer.UnitySendMessage(unityGameObject, "OnAgeSignalsDebug", "[addOnSuccessListener]");
			})
			.addOnFailureListener(exception -> {
				
				String message = exception.getMessage();
				if (message == null) {
					message = exception.toString();
				}
				
				UnityPlayer.UnitySendMessage(
					unityGameObject,
					"OnAgeSignalsError",
					message
				);
				
				UnityPlayer.UnitySendMessage(unityGameObject, "OnAgeSignalsDebug", "[addOnFailureListener]");
			});
	}
	
	private static void resultInvalid(String unityGameObject, String reason) {
		
		if (reason == null) {
			reason = "NULLREASON";
		}
		
		String message = "INVALID @ " + reason + "|" + "-1" + "|" + "-1";
		
		UnityPlayer.UnitySendMessage(
			unityGameObject,
			"OnAgeSignalsError",
			"ERROR: " + message
		);
		
		UnityPlayer.UnitySendMessage(
			unityGameObject,
			"OnAgeSignalsResult",
			message
		);
	}
}