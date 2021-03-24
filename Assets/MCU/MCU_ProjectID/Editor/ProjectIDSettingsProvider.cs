using System;

using UnityEngine;

using UnityEditor;
using UnityEditor.Callbacks;

namespace MCU.ProjectID {
    /// <summary>
    /// Manage the displaying of the Project ID Settings within the unity project settings
    /// </summary>
    public static class ProjectIDSettingsProvider {
        /*----------Types----------*/
        //PRIVATE

        /// <summary>
        /// Store the different style elements that will be used to display settings
        /// </summary>
        private sealed class Styles {
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////-------------------------------------Labels-------------------------------------//////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static GUIContent projectIDLabel = new GUIContent(
                "Project ID",
                "The unique ID that is assigned to this project to be able to differentiate elements"
            );
            public static GUIContent lockLabel = new GUIContent(
                "Lock",
                "Lock the Project ID field so that it can't be modified"
            );
            public static GUIContent unlockLabel = new GUIContent(
                "Unlock",
                "Unlock the Project ID field so that it can be modified"
            );
            public static GUIContent newLabel = new GUIContent(
                "New",
                "Generate a new ID for use with this project"
            );

            ////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////------------------------------------Layouts-------------------------------------//////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////

            public static GUILayoutOption newBtnWidth  = GUILayout.Width(50f);
            public static GUILayoutOption lockBtnWidth = GUILayout.Width(50f);
        }

        /// <summary>
        /// Override the default editor to redirect the user to the project settings for modification
        /// </summary>
        [CustomEditor(typeof(ProjectIDSettings))]
        private sealed class ProjectIDSettingsEditor : Editor {
            /*----------Variables----------*/
            //PRIVATE

            /// <summary>
            /// Store the label that will be used for the button
            /// </summary>
            private GUIContent redirectBtn = new GUIContent("Open in Project Settings");

            /*----------Functions----------*/
            //PRIVATE

            /// <summary>
            /// If trying to open a settings asset, take the user to the settings screen
            /// </summary>
            /// <param name="instanceID">The ID of the asset that was attempted to be opened</param>
            /// <param name="line">[Ignored] The line to open the asset to</param>
            /// <returns>Returns true if the asset opening was handled by this function</returns>
            [OnOpenAsset]
            private static bool OnOpenAsset(int instanceID, int line) {
                // If this object is a Settings object...
                if (EditorUtility.InstanceIDToObject(instanceID) is ProjectIDSettings) {
                    OpenProjectIDSettings();
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Open the Project ID settings display within the project settings
            /// </summary>
            private static void OpenProjectIDSettings() { SettingsService.OpenProjectSettings(PROVIDER_DISPLAY_LOCATION); }

            //PUBLIC

            /// <summary>
            /// Offer a button to quickly take the user to the project ID settings
            /// </summary>
            public override void OnInspectorGUI() {
                if (GUILayout.Button(redirectBtn))
                    OpenProjectIDSettings();
            }
        }

        /*----------Variables----------*/
        //CONST

        /// <summary>
        /// The location within the project settings where this provider shall be offered
        /// </summary>
        private const string PROVIDER_DISPLAY_LOCATION = "Project/Project ID";

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Initialise the Settings Provider that will be used to modify the settings 
        /// </summary>
        /// <returns>Returns the Settings Provider instance to be used</returns>
        [SettingsProvider]
        public static SettingsProvider CreateProjectIDSettingsProvider() {
            // If no settings are available, no need for a provider
            if (!IsSettingsAvailable())
                return null;

            // Capture the values that will be used to display the elements
            ProjectIDSettings idSettings = null;
            SerializedObject serialSettings = null;
            SerializedProperty projectIDProp = null;
            bool isUnlocked = false;

            // Create the provider object that will be displayed
            SettingsProvider provider = new SettingsProvider(PROVIDER_DISPLAY_LOCATION, SettingsScope.Project)
            {
                label = Styles.projectIDLabel.text,
                guiHandler = (searchContext) => {
                    //Ensure that there is an object to be displayed
                    if (!idSettings) {
                        // Get the settings object to be shown
                        idSettings = ProjectIDSettings.Settings;

                        // If there was a settings object to be found, grab the required values
                        if (idSettings) {
                            serialSettings = new SerializedObject(idSettings);
                            projectIDProp = serialSettings.FindProperty("projectID");
                        }

                        // Otherwise, there can't be anything displayed
                        else {
                            EditorGUILayout.LabelField("Unable to load Project ID Settings asset for modification", EditorStyles.boldLabel);
                            return;
                        }
                    }

                    // Update the serial representation in case of script modifications
                    else serialSettings.UpdateIfRequiredOrScript();

                    // Display these settings along a single line
                    EditorGUILayout.BeginHorizontal(); {
                        // Store the prev UI enabled state
                        bool prevState = GUI.enabled;
                        GUI.enabled = isUnlocked;

                        // If any changes are made, they need to be applied
                        EditorGUI.BeginChangeCheck();

                        // Display the Project ID field for reference
                        EditorGUILayout.PropertyField(projectIDProp, Styles.projectIDLabel, true);

                        // If the user wants to generate a new ID 
                        if (GUILayout.Button(Styles.newLabel, Styles.newBtnWidth)) {
                            GUIUtility.hotControl =
                            GUIUtility.keyboardControl = 0;
                            projectIDProp.stringValue = Guid.NewGuid().ToString();
                        }

                        // If anything was modified, apply the change
                        if (EditorGUI.EndChangeCheck())
                            serialSettings.ApplyModifiedProperties();

                        // Restore the previous UI state
                        GUI.enabled = prevState;

                        // Offer a button to change the lock state of the elements
                        if (GUILayout.Button(isUnlocked ? Styles.lockLabel : Styles.unlockLabel, Styles.lockBtnWidth))
                            isUnlocked = !isUnlocked;
                    } EditorGUILayout.EndHorizontal();
                },

                // Use the display style elements for the keywords
                keywords = SettingsProvider.GetSearchKeywordsFromGUIContentProperties<Styles>()
            };

            return provider;
        }

        /// <summary>
        /// Check if there is a settings object that can be displayed
        /// </summary>
        /// <returns>Returns true if there is a settings object available</returns>
        public static bool IsSettingsAvailable() {
            try {
                return ProjectIDSettings.Settings;
            } catch (Exception exec) {
                Debug.LogErrorFormat("Unable to load Project ID Settings. ERROR: {0}", exec);
                return false;
            }
        }
    }
}