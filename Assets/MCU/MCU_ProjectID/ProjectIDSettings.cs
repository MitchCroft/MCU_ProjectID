using System;

using UnityEngine;

#if UNITY_EDITOR
using System.IO;

using UnityEditor;
#endif

namespace MCU.ProjectID {
    /// <summary>
    /// Store the different settings that are related to the ID of the project
    /// </summary>
    [CreateAssetMenu(menuName = "MCU/Project ID Settings")]
    public sealed class ProjectIDSettings : ScriptableObject {
        /*----------Variables----------*/
        //CONST

#if UNITY_EDITOR
        /// <summary>
        /// The default location that will have an asset created if none could be found for use
        /// </summary>
        private const string DEFAULT_SETTINGS_LOCATION = "Assets/Resources/MCU/ProjectIDSettings.asset";
#endif

        //INVISIBLE

        /// <summary>
        /// Store the singleton instance of the Project ID settings 
        /// </summary>
        private static ProjectIDSettings instance;

        //VISIBLE

        [SerializeField, Tooltip("The unique ID that will be used to represent this project")]
        private string projectID = Guid.NewGuid().ToString();

        /*----------Properties----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the instance of the settings asset that will be referenced
        /// </summary>
        public static ProjectIDSettings Settings {
            get {
                // Check if the asset needs to be loaded
                if (!instance) {
                    // Look in the resources for an ID settings asset that can be used
                    ProjectIDSettings[] settings = Resources.LoadAll<ProjectIDSettings>(string.Empty);

                    // There can't be more then one settings object
                    if (settings.Length > 1)
                        throw new Exception(string.Format("Found {0} Project ID Settings objects within the project resources. There can only be 1", settings.Length));
                    else if (settings.Length == 1)
                        instance = settings[0];

#if UNITY_EDITOR
                    // If no instance could be found, create one for the project
                    if (!instance) {
                        // Create a new instance of the asset
                        instance = ScriptableObject.CreateInstance<ProjectIDSettings>();

                        // Store the asset within the project for re-use
                        Directory.CreateDirectory(Path.GetDirectoryName(DEFAULT_SETTINGS_LOCATION));
                        AssetDatabase.CreateAsset(instance, DEFAULT_SETTINGS_LOCATION);
                        AssetDatabase.SaveAssets();

                        // If the editor isn't running, save and refresh the assets
                        if (!EditorApplication.isPlaying) 
                            AssetDatabase.Refresh();
                    }
#endif
                }

                // There must be an instance to return, otherwise there will be a problem
                return instance ?? throw new Exception("No Project ID Settings asset could be identified for use");
            }
        }

        /// <summary>
        /// The unique ID that will be used to represent this project
        /// </summary>
        public string ProjectID {
            get { return projectID; }
            set { projectID = (value == null ? string.Empty : value); }
        }

        /// <summary>
        /// Retrieve a Hash ID that combines <see cref="ProjectID"/> and <see cref="Application.version"/>
        /// </summary>
        /// <remarks>
        /// This can be used to differentiate between different versions of the same project as needed
        /// </remarks>
        public int VersionHash {
            get {
                unchecked {
                    int hash = 17;
                    hash = hash * 31 + projectID.GetHashCode();
                    hash = hash * 31 + Application.version.GetHashCode();
                    return hash;
                }
            }
        }

        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Generate a new Project ID for this asset
        /// </summary>
        public void GenerateNewID() { projectID = Guid.NewGuid().ToString(); }

        /// <summary>
        /// Use the internal project ID as the string representation of this object
        /// </summary>
        /// <returns>Returns the internal project ID</returns>
        public override string ToString() { return projectID; }
    }
}