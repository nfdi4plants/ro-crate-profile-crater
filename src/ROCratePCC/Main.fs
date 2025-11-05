namespace ROCratePCC


open ARCtrl.ROCrate
open ARCtrl.Json
open Fable.Core


type ResourceDescriptorType =
    | Specification
    | Constraint
    | Guidance
    | Example
    // object id * url to role
    | Other of string*string

    with

    member this.ID =
        match this with
        | Specification -> "#hasSpecification"
        | Constraint -> "#hasConstraint"
        | Guidance -> "#hasGuidance"
        | Example -> "#hasExample"
        | Other(id, _) -> id

    member this.Role =
        match this with
        | Specification -> "http://www.w3.org/ns/dx/prof/role/specification"
        | Constraint -> "http://www.w3.org/ns/dx/prof/role/constraints"
        | Guidance -> "http://www.w3.org/ns/dx/prof/role/guidance"
        | Example -> "http://www.w3.org/ns/dx/prof/role/example"
        | Other(_, role) -> role

[<AttachMembers>]
type Author(orcid : string, name : string) as n =

    inherit LDNode(id = $"https://orcid.org/{orcid}", schemaType = ResizeArray [LDPerson.schemaType])

    do LDDataset.setNameAsString(n, name)

[<AttachMembers>]
type UsedType(iri : string, name : string) as n =

    inherit LDNode(id = iri, schemaType = ResizeArray [LDDefinedTerm.schemaType])

    do LDDataset.setNameAsString(n, name)

[<AttachMembers>]
type License(iri : string, name : string) as n =
    inherit LDNode(id = iri, schemaType = ResizeArray [LDCreativeWork.schemaType])
    do LDDataset.setNameAsString(n, name)

[<AttachMembers>]
type TextualResource(name : string, filePath : string, encodingFormat : string, ?rootDataEntityId) as n =
    inherit LDNode(id = filePath, schemaType = ResizeArray [LDFile.schemaType])

    do 
        LDDataset.setNameAsString(n, name)
        LDFile.setEncodingFormatAsString(n, encodingFormat)
        match rootDataEntityId with
        | Some id -> n.SetProperty(LDFile.about, LDRef(id = id))
        | None -> ()

[<AttachMembers>]
type ResourceDescriptor(textualResources : ResizeArray<TextualResource>, resourceDescriptorType : ResourceDescriptorType) as n =
    inherit LDNode(id = resourceDescriptorType.ID, schemaType = ResizeArray ["http://www.w3.org/ns/dx/prof/ResourceDescriptor"])
    do
        //let artifacts = textualResources |> Seq.map (fun tr -> LDRef(tr.Id)) |> ResizeArray
        //n.SetProperty("http://www.w3.org/ns/dx/prof/hasArtifact", artifacts)
        n.SetProperty("http://www.w3.org/ns/dx/prof/hasRole", LDRef(resourceDescriptorType.Role))
        n.SetProperty("http://www.w3.org/ns/dx/prof/hasArtifact", textualResources)

[<AttachMembers>]
type Specification(textualResources : ResizeArray<TextualResource>) =
    inherit ResourceDescriptor(textualResources = textualResources, resourceDescriptorType = ResourceDescriptorType.Specification)

[<AttachMembers>]
type Constraint(textualResources : ResizeArray<TextualResource>) =
    inherit ResourceDescriptor(textualResources = textualResources, resourceDescriptorType = ResourceDescriptorType.Constraint)

[<AttachMembers>]
type Guidance(textualResources : ResizeArray<TextualResource>) =
    inherit ResourceDescriptor(textualResources = textualResources, resourceDescriptorType = ResourceDescriptorType.Guidance)

[<AttachMembers>]
type Example(textualResources : ResizeArray<TextualResource>) =
    inherit ResourceDescriptor(textualResources = textualResources, resourceDescriptorType = ResourceDescriptorType.Example)

[<AttachMembers>]
type RootDataEntity(id : string, name : string, description : string, license: License, usedTypes : ResizeArray<UsedType>, resourceDescriptors : ResizeArray<ResourceDescriptor>, authors : ResizeArray<Author>) as n =
    inherit LDNode(id = id, schemaType = ResizeArray [LDDataset.schemaType; "http://www.w3.org/ns/dx/prof/Profile"])
    do
        let textualResources : ResizeArray<LDNode> =
            ResizeArray [
                for rd in resourceDescriptors do
                    yield! rd.GetPropertyNodes("http://www.w3.org/ns/dx/prof/hasArtifact")
            ]
        let hasParts : List<LDNode> = [for tr in textualResources do tr; for ut in usedTypes do ut]
        LDDataset.setLicenseAsCreativeWork(n, license)
        LDDataset.setNameAsString(n, name)
        LDDataset.setDescriptionAsString(n, description)
        n.SetProperty("http://schema.org/author", authors)
        LDDataset.setHasParts(n, ResizeArray hasParts)
        n.SetProperty("http://www.w3.org/ns/dx/prof/hasResource", resourceDescriptors)

[<AttachMembers>]
type Profile(rootDataEntity : RootDataEntity, ?license : License, ?roCrateSpec : string) as n =
    inherit LDNode(id = "ro-crate-metadata.json", schemaType = ResizeArray [LDCreativeWork.schemaType])
    do
        LDDataset.setAbouts(n, ResizeArray [rootDataEntity :> LDNode])
        if license.IsSome then LDDataset.setLicenseAsCreativeWork(n, license.Value)
        let roCrateSpec = Option.defaultValue "https://w3id.org/ro/crate/1.2" roCrateSpec
        n.SetProperty("http://purl.org/dc/terms/conformsTo", roCrateSpec)

    member this.ToROCrateJsonString(?spaces : int) =
        let context = Context.initV1_2DRAFT()
        this.Compact_InPlace(context, false)
        let graph = this.Flatten()
        graph.SetContext(context)
        graph.ToROCrateJsonString(?spaces = spaces)





