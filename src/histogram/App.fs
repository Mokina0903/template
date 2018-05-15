namespace histogram

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI
open Aardvark.UI.Primitives
open Aardvark.Base.Rendering
open histogram.Model

type Message =
    | ToggleModel
    | CameraMessage of CameraControllerMessage

module App =
   

    let update (m : Model) (msg : Message) =
        match msg with
            | ToggleModel -> 
                match m.currentModel with
                    | Box -> { m with currentModel = Sphere }
                    | Sphere -> { m with currentModel = Box }

            | CameraMessage msg ->
                { m with cameraState = CameraController.update m.cameraState msg }

    

    let view (m : MModel) =

        let helper (data : Map<string, float>) : DomNode<'a> = 
            let newStuff = 
                    data |> Map.toList |> List.map (fun x -> 
                        let (name, value) = x
                        value
                        )
            Svg.rect [attribute "width" "50px"; attribute "height" "50px"]

        let dynamicGui =
            alist {
                let! data = m.data
                let content = helper data
                yield Svg.svg [attribute "width" "600px"; attribute "height" "400px"] [content]
           }

        let frustum = 
            Frustum.perspective 60.0 0.1 100.0 1.0 
                |> Mod.constant

        let sg =
            m.currentModel |> Mod.map (fun v ->
                match v with
                    | Box -> Sg.box (Mod.constant C4b.Red) (Mod.constant (Box3d(-V3d.III, V3d.III)))
                    | Sphere -> Sg.sphere 5 (Mod.constant C4b.Green) (Mod.constant 1.0)
            )
            |> Sg.dynamic
            |> Sg.shader {
                do! DefaultSurfaces.trafo
                do! DefaultSurfaces.simpleLighting
            }

        let att =
            [
                style "position: fixed; left: 0; top: 0; width: 100%; height: 100%"
            ]

        body [] [
            Incremental.div AttributeMap.empty dynamicGui
            //CameraController.controlledControl m.cameraState CameraMessage frustum (AttributeMap.ofList att) sg
            //
            //div [style "position: fixed; left: 20px; top: 20px"] [
            //    button [onClick (fun _ -> ToggleModel)] [text "Toggle Model"]
            //]
            //
        ]

    let app =
        let data = Map.empty
        {
            initial =  { data = data; currentModel = Box; cameraState = CameraController.initial }
            update = update
            view = view
            threads = fun _ -> ThreadPool.empty
            unpersist = Unpersist.instance
        }