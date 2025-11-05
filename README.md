# RO-Crate Profile Crate Creator

Polyglot library for creation of RO-Crate Profile Crates according to [the official documentation](https://www.researchobject.org/ro-crate/specification/1.2/profiles.html#profile-crate), and based on the examples of [profile run crate](https://www.researchobject.org/workflow-run-crate/profiles/workflow_run_crate/ro-crate-metadata.json) and [workflow crate](https://about.workflowhub.eu/Workflow-RO-Crate/ro-crate-metadata.json).

# Documentation

## FSharp

```fsharp
open ROCratePCC

let types : ResizeArray<UsedType> = ResizeArray [
    UsedType(iri = "https://schema.org/CreativeWork", name = "CreativeWork");
    UsedType(iri = "http://www.w3.org/ns/dx/prof/Profile", name = "Profile");
]

let authors : ResizeArray<Author> = ResizeArray [
    Author(orcid = "0000-0002-5526-71389", name = "Florian Wetzels");
    Author(orcid = "0000-0003-1945-6342", name = "Heinrich Lukas Weil");
]

let version = "1.0.0-draft.2"

let id = $"https://github.com/nfdi4plants/isa-ro-crate-profile/tree/{version}/profile"

let name = "ISA RO-Crate Profile"

let description = "An RO-Crate profile for representing ISA data in Research Object Crates (RO-Crates). This profile defines how to represent ISA Investigation, Study, and Assay data using RO-Crate metadata."

let license = License(iri = "https://mit-license.org/", name = "MIT License")

let specifications = ResizeArray[
    TextualResource(
        name = "ISA RO-Crate Profile description",
        filePath = "isa_ro_crate.md",
        encodingFormat = "text/markdown",
        rootDataEntityId = id
    )
]


let resourceDescriptors = ResizeArray [
    Specification(specifications) :> ResourceDescriptor
]

let rootEntity = 
    RootDataEntity(
        id = id,
        name = name,
        description = description,
        license = license,
        usedTypes = types,
        resourceDescriptors = resourceDescriptors,
        authors = ResizeArray authors
    )

let profile = 
    Profile(
        rootEntity,
        license = license
    )

let string = profile.ToROCrateJsonString(spaces = 2)

System.IO.File.WriteAllText("profile/ro-crate-metadata.json", string)
```
