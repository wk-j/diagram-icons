open System.IO
open System.Text

type IconInfo = {
    Name: string
    Namespace: string
    Path: string
}

let topDir =
    let dir = DirectoryInfo("resources")
    dir.GetDirectories()

let toPascal (input: string) =
    let info = System.Globalization.CultureInfo.CurrentCulture.TextInfo
    let title = info.ToTitleCase(input.Replace("-", " ").Replace(".png", ""))
    title.Replace(" ", "")

let getIconInfo(dir: DirectoryInfo) =
    let files =
        dir.GetFiles("*.png", SearchOption.AllDirectories)

    let data = ResizeArray<IconInfo>()
    let currentDir = DirectoryInfo(".").FullName

    for item in files do
        let name = item.Name
        let cateogry = item.Directory.Name
        let vendor = item.Directory.Parent.Name
        let fullPath = item.FullName
        let fullDirName = item.Directory.FullName

        let ns =
            fullDirName
                .Replace(currentDir, "")
                .Replace("resources", "")
                .Replace("/", ".").ToLower()
                .Replace(".png", "")
                .TrimStart('.')

        let path =
            fullPath
                .Replace(currentDir, "")
                .TrimStart('/')

        data.Add
            { Name = name |> toPascal
              Namespace = ns
              Path = $"../{path}" }
    data

let createMD (icons: ResizeArray<IconInfo>) =
    let builder = StringBuilder()

    for item in icons do
        // let fmt = $"{item.Category}|{item.Name}|![]({item.Path} =50x50)\n"
        let fmt = $"`diagram.{item.Namespace}`|{item.Name}|<img src=\"{item.Path}\" width=\"50px\" />\n"
        builder.Append(fmt) |> ignore

    builder.ToString()

for item in topDir do
    let name = item.Name
    let template = File.ReadAllText("template/List.md")
    let target = $"list/{name}.md"
    let infos = getIconInfo item
    let md = createMD infos


    let value =
        template
            .Replace("{_title_}", name)
            .Replace("{_icons_}", md)

    File.WriteAllText(target, value) |> ignore