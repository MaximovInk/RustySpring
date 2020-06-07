namespace MaximovInk
{
    public interface IMod
    {
        bool IsEnabled { get; set; }

        void Load();

        void Init();

        void OnEnable();

        void OnDisable();

        void OnUnload();
    }
}