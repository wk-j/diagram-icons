open System.IO
open System.Text

type IconInfo = {
    Name: string
    Category: string
    Vendor: string
    Path: string
}

let topDir =
    let dir = DirectoryInfo("resources")
    dir.GetDirectories()

let getIconInfo(dir: DirectoryInfo) =
    let files =
        dir.GetFiles("*.png", SearchOption.AllDirectories)

    let data = ResizeArray<IconInfo>()

    for item in files do
        let name = item.Name
        let cateogry = item.Directory.Name
        let vendor = item.Directory.Parent.Name

        data.Add
            { Name = name
              Category = cateogry
              Vendor = vendor
              Path = $"../resources/{vendor}/{cateogry}/{name}" }
    data

let createMD (icons: ResizeArray<IconInfo>) =
    let builder = StringBuilder()

    for item in icons do
        let fmt = $"{item.Category}|{item.Name}|![]({item.Path} =50x50)\n"
        builder.Append(fmt)

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

    File.WriteAllText(target, value)

    ()