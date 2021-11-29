open System
open System.Diagnostics

open System.Net.Http

open System.Text.RegularExpressions
open System.Threading.Tasks


let client = new HttpClient()

let load (url: string) =
    task {
        Console.WriteLine $"Starting to download {url}"

        let! resp = client.GetAsync(url)
        return! resp.Content.ReadAsStringAsync()
    }

let regex =
    Regex("href=\"(https:[^\"]+)\"", RegexOptions.IgnoreCase ||| RegexOptions.Compiled)

let extractLinks text =
    regex.Matches(text)
    |> Seq.map (fun x -> x.Groups.[1].Value)
    |> Set.ofSeq


let first () =
    task {
        let! res = load "https://www.bbc.com/news/world"
        return extractLinks res
    }

let second (links: Task<Set<string>>) =
    task {
        let! links = links
        Console.WriteLine $"Extracted {links.Count} links, starting watch..."
        let watch = Stopwatch.StartNew()
        let newLinks = links |> Seq.map load
        let! results = Task.WhenAll(newLinks)

        let totalLength =
            results |> Seq.fold (fun x y -> x + y.Length) 0

        watch.Stop()
        Console.WriteLine $"Fetched {totalLength} chars in {watch.ElapsedMilliseconds} ms"
    }

first() |> second |> Task.WaitAll
