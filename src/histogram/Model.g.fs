namespace histogram.Model

open System
open Aardvark.Base
open Aardvark.Base.Incremental
open histogram.Model

[<AutoOpen>]
module Mutable =

    
    
    type MModel(__initial : histogram.Model.Model) =
        inherit obj()
        let mutable __current : Aardvark.Base.Incremental.IModRef<histogram.Model.Model> = Aardvark.Base.Incremental.EqModRef<histogram.Model.Model>(__initial) :> Aardvark.Base.Incremental.IModRef<histogram.Model.Model>
        let _data = ResetMod.Create(__initial.data)
        let _currentModel = ResetMod.Create(__initial.currentModel)
        let _cameraState = Aardvark.UI.Primitives.Mutable.MCameraControllerState.Create(__initial.cameraState)
        
        member x.data = _data :> IMod<_>
        member x.currentModel = _currentModel :> IMod<_>
        member x.cameraState = _cameraState
        
        member x.Current = __current :> IMod<_>
        member x.Update(v : histogram.Model.Model) =
            if not (System.Object.ReferenceEquals(__current.Value, v)) then
                __current.Value <- v
                
                ResetMod.Update(_data,v.data)
                ResetMod.Update(_currentModel,v.currentModel)
                Aardvark.UI.Primitives.Mutable.MCameraControllerState.Update(_cameraState, v.cameraState)
                
        
        static member Create(__initial : histogram.Model.Model) : MModel = MModel(__initial)
        static member Update(m : MModel, v : histogram.Model.Model) = m.Update(v)
        
        override x.ToString() = __current.Value.ToString()
        member x.AsString = sprintf "%A" __current.Value
        interface IUpdatable<histogram.Model.Model> with
            member x.Update v = x.Update v
    
    
    
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Model =
        [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
        module Lens =
            let data =
                { new Lens<histogram.Model.Model, Microsoft.FSharp.Collections.Map<System.String,System.Double>>() with
                    override x.Get(r) = r.data
                    override x.Set(r,v) = { r with data = v }
                    override x.Update(r,f) = { r with data = f r.data }
                }
            let currentModel =
                { new Lens<histogram.Model.Model, histogram.Model.Primitive>() with
                    override x.Get(r) = r.currentModel
                    override x.Set(r,v) = { r with currentModel = v }
                    override x.Update(r,f) = { r with currentModel = f r.currentModel }
                }
            let cameraState =
                { new Lens<histogram.Model.Model, Aardvark.UI.Primitives.CameraControllerState>() with
                    override x.Get(r) = r.cameraState
                    override x.Set(r,v) = { r with cameraState = v }
                    override x.Update(r,f) = { r with cameraState = f r.cameraState }
                }
