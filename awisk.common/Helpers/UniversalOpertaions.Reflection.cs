namespace awisk.common.Helpers
{
    public static partial class UniversalOpertaions
    {
        public static Dictionary<string, object?> ToDictionary<T>(this T obj)
            => typeof(T).GetProperties().ToDictionary(p => p.Name, p => p.GetValue(obj, null));

        public static void CopyProperties<TSrc, TDest>(this TSrc source, TDest dest)
        {
            if (source == null || dest == null)
            {
                return;
            }

            var srcProps = typeof(TSrc).GetProperties();
            var destProps = typeof(TDest).GetProperties().ToDictionary(p => p.Name);
            foreach (var sp in srcProps)
            {
                if (destProps.TryGetValue(sp.Name, out var dp) && dp.CanWrite)
                {
                    dp.SetValue(dest, sp.GetValue(source));
                }
            }
        }
    }
}
