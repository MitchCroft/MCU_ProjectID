#if MCU_REG
namespace MCU.ProjectID {
    /// <summary>
    /// Provide access to the Project ID data elements via the adapter interface
    /// </summary>
    [Registry.MCUAdapter("project_id", "1.0.0", "PROJ_ID")]
    public static class ProjectIDRegistryAdapter {
        /*----------Functions----------*/
        //PUBLIC

        /// <summary>
        /// Retrieve the current Project ID that is assigned to this project
        /// </summary>
        /// <returns>Returns the ProjectID defined in the <see cref="ProjectIDSettings.Settings"/> asset</returns>
        [Registry.MCUAdapterMethod("project_id")]
        public static string GetProjectID() { return ProjectIDSettings.Settings.ProjectID; }

        /// <summary>
        /// Retrieve the Project ID/Version hash currently in use
        /// </summary>
        /// <returns>Returns the ProjectID defined in the <see cref="ProjectIDSettings.VersionHash"/> asset</returns>
        [Registry.MCUAdapterMethod("project_version_hash")]
        public static int GetProjectIDVersionHash() { return ProjectIDSettings.Settings.VersionHash; }
    }
}
#endif