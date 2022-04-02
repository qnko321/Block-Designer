public static class IdNumberGenerator
{
    private static int _faceIndex = 1;
    private static int _childElementIndex = 1;

    public static string GenerateFaceId()
    {
        string _id = "0-" + _faceIndex.ToString("D6");
        _faceIndex++;
        return _id;
    }

    public static string GenerateVertexId(string _faceId)
    {
        string _id = $"1-{_faceId}-{_childElementIndex:D6}";
        _childElementIndex++;
        return _id;
    }
    
    public static string GenerateTriangleId(string _faceId)
    {
        string _id = $"2-{_faceId}-{_childElementIndex:D6}";
        _childElementIndex++;
        return _id;
    }
}