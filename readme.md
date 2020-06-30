# Xml Summary Comments Generator VS Extension

## About

[Visual Studio Extension](https://marketplace.visualstudio.com/items?itemName=loukaspd.XmlCommentsGenerator)  
Simple Visual Studio Extension for automatically adding `[DataMember]` and comment summary above class properties based on their names


> Input
```C#
public class ClassName {
    public string GreekAddress { get; set; }
}
```

> Output
```C#
public class ClassName {
    /// <summary>
    /// Greek Address
    /// </summary>
    [DataMember(Name = "greekAddress")]
    public string GreekAddress { get; set; }
}
```

