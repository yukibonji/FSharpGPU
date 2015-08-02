﻿(*This file is part of FSharpGPU.

FSharpGPU is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

FSharpGPU is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with FSharpGPU.  If not, see <http://www.gnu.org/licenses/>.
*)

(* Copyright © 2015 Philip Curzon *)

namespace NovelFS.FSharpGPU

/// Error handling functions
module private DeviceError =
    let raiseNotSupportedOperation() =
        raise <| System.NotSupportedException ("Performing operations directly on the CUDA data is not supported.  The operation should be called as part of a Quatation.")

/// Whether a value was generated by the user or automatically
type internal GenerationMethod =
    |UserGenerated
    |AutoGenerated

/// Whether the array points directly to the list of values or whether there is some offset
type internal ArraySpecification =
    |FullArray
    |OffsetSubarray of int

/// Container for Device Arrays
type ComputeArray internal (arrayType : ComputeResult, cudaPtr : System.IntPtr, length : int, arraySpec : ArraySpecification, generationMethod : GenerationMethod) = 
    let mutable isDisposed = false;
    let cleanup () =
        match arraySpec with
        |FullArray ->
            match isDisposed with
            |true -> ()
            |false -> 
                isDisposed <- true
                DeviceInterop.freeArray(cudaPtr) |> DeviceInterop.cudaCallWithExceptionCheck
        |OffsetSubarray n -> ()
    member internal this.ArrayType = arrayType
    member internal this.CudaPtr = cudaPtr
    member internal this.GenMethod = generationMethod
    member internal this.Offset = 
        match arraySpec with
        |FullArray -> 0
        |OffsetSubarray offs -> offs
    member this.Length = length
    member this.Dispose() = cleanup()
    override this.Finalize() = cleanup()
        
/// Result of breaking down an expression is either an array or just a primitive
and internal ComputeResult =
    |ResComputeArray of ComputeArray
    |ResComputeFloat of float
    |ResComputeFloat32 of float32
    |ResComputeBool of bool
    member this.Length =
        match this with
        |ResComputeArray devArray -> devArray.Length
        |ResComputeFloat devDouble -> sizeof<float>
        |ResComputeFloat32 devFloat -> sizeof<float32>
        |ResComputeBool devBool -> sizeof<bool>
/// Functions to query information stored in the device array
module internal DeviceArrayInfo = 
    let length = 
        function
        |ResComputeArray devArray -> devArray.Length
        |ResComputeFloat devDouble -> sizeof<float>
        |ResComputeFloat32 devFloat -> sizeof<float32>
        |ResComputeBool devBool -> sizeof<int>

module TypeHelper =
    let raiseNotSupported() = raise <| System.NotSupportedException("This method should not be called directly, it should be used as part of a quotation.")

//
// GPU Types
// ---------
// These types do not and should not have implemented functionality.  They exist only to allow the F# type-checker to enforce the correctness of expressions.

/// A type that exists on the GPU
type IGPUType = interface end

/// A bool on the GPU
type devicebool = class
    interface IGPUType
    end

/// A (double precision) floating point number on the GPU
type devicefloat =
    interface IGPUType
    static member ( + ) (flt1 : devicefloat, flt2 : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member ( + ) (flt1 : devicefloat, flt2 : float) : devicefloat = TypeHelper.raiseNotSupported()
    static member ( + ) (flt1 : float, flt2 : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member ( - ) (flt1 : devicefloat, flt2 : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member ( - ) (flt1 : devicefloat, flt2 : float) : devicefloat = TypeHelper.raiseNotSupported()
    static member ( - ) (flt1 : float, flt2 : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member ( * ) (flt1 : devicefloat, flt2 : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member ( * ) (flt1 : devicefloat, flt2 : float) : devicefloat = TypeHelper.raiseNotSupported()
    static member ( * ) (flt1 : float, flt2 : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member ( / ) (flt1 : devicefloat, flt2 : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member ( / ) (flt1 : devicefloat, flt2 : float) : devicefloat = TypeHelper.raiseNotSupported()
    static member ( / ) (flt1 : float, flt2 : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Pow (flt1 : devicefloat, flt2 : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Pow (flt1 : devicefloat, flt2 : float) : devicefloat = TypeHelper.raiseNotSupported()
    static member Pow (flt1 : float, flt2 : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Sin (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Cos (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Tan (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Sinh (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Cosh (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Tanh (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Asin (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Acos (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Atan (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Sqrt (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Log (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Log10 (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member Exp (flt : devicefloat) : devicefloat = TypeHelper.raiseNotSupported()
    static member CompareTo (flt1 : devicefloat, flt2 : float) = TypeHelper.raiseNotSupported()
    static member CompareTo (flt1 : devicefloat, flt2 : devicefloat) = TypeHelper.raiseNotSupported()
    static member CompareTo (flt1 : float, flt2 : devicefloat) = TypeHelper.raiseNotSupported()

[<AutoOpen>]
module ComputeOperators =
    /// device capable greater than operator
    let inline (.>.) (val1 : ^a) (val2 : ^b) : devicebool =
        let compare = ((^a or ^b)  : (static member CompareTo : ^a * ^b -> devicebool) (val1, val2))
        TypeHelper.raiseNotSupported()
    /// device capable greater than or equal operator
    let inline (.>=.) (val1 : 'a) (val2 : 'b) : devicebool =
        let compare = ((^a or ^b)  : (static member CompareTo : ^a * ^b -> devicebool) (val1, val2))
        TypeHelper.raiseNotSupported()
    /// device capable less than operator
    let inline (.<.) (val1 : 'a) (val2 : 'b) : devicebool =
        let compare = ((^a or ^b)  : (static member CompareTo : ^a * ^b -> devicebool) (val1, val2))
        TypeHelper.raiseNotSupported()
    /// device capable less than or equal operator
    let inline (.<=.) (val1 : 'a) (val2 : 'b) : devicebool =
        let compare = ((^a or ^b)  : (static member CompareTo : ^a * ^b -> devicebool) (val1, val2))
        TypeHelper.raiseNotSupported()

    let ( .=. ) val1 val2 : devicebool  = TypeHelper.raiseNotSupported()
    let ( .<>. ) val1 val2 : devicebool  = TypeHelper.raiseNotSupported()
    let ( .&&. ) (val1 : devicebool) (val2 : devicebool) : devicebool  = TypeHelper.raiseNotSupported()
    let ( .||. ) (val1 : devicebool) (val2 : devicebool) : devicebool  = TypeHelper.raiseNotSupported()

/// An array of items stored on the GPU
type devicearray<'a when 'a :> IGPUType>(devArray : ComputeArray) = 
    member internal this.DeviceArray = devArray
