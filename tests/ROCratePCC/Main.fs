module ROCratePCC.Tests

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.Conversion
open ARCtrl.Process
open TestingUtils
open ARCtrl.FileSystem
open Fable.Pyxpecto

let tests_PlaceHolder = 
    testList "PlaceHolder" [
        testCase "Positive" <| fun _ ->
            Expect.isTrue true "dawdaw"
        ptestCase "Negative" <| fun _ ->
            Expect.isFalse true "dawdaw"
    ]



let all = 
    testList "ArcROCrateConversion" [
        tests_PlaceHolder
    ]

#if !TESTS_ALL
[<EntryPoint>]
#endif
let main argv = Pyxpecto.runTests [||] all