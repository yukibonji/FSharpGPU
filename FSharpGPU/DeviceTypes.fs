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
module DeviceError =
    /// Helper function for raising exceptions in stub device function declerations
    let raiseNotSupportedOperation() =
        raise <| System.NotSupportedException ("Performing operations directly on the CUDA data is not supported.  The operation should be called as part of a Quotation.")

/// Whether a value was generated by the user or automatically
type internal GenerationMethod =
    |UserGenerated
    |AutoGenerated

/// Whether the array points directly to the list of values or whether there is some offset
type internal ArraySpecification =
    |FullArray
    |OffsetSubarray of int

/// Container for Device Arrays
type internal ComputeArray internal (arrayType : ComputeDataType, cudaPtr : System.IntPtr, length : int, arraySpec : ArraySpecification, generationMethod : GenerationMethod) = 
    let mutable isDisposed = false
    let cleanup () =
        match arraySpec with
        |FullArray ->
            match isDisposed with
            |true -> ()
            |false -> 
                isDisposed <- true
                DeviceInterop.freeArray(cudaPtr) |> DeviceInterop.cudaCallWithExceptionCheck
        |_ -> ()
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

    interface System.IDisposable with
        member this.Dispose() = this.Dispose()
        
/// Result of breaking down an expression is either an array or just a primitive
and internal ComputeResult =
    |ResComputeTupleArray of ComputeResult list
    |ResComputeArray of ComputeArray
    |ResComputeFloat of float
    |ResComputeFloat32 of float32
    |ResComputeBool of bool

    interface System.IDisposable with
        member this.Dispose() =
            match this with
            |ResComputeArray ca -> 
                match ca.GenMethod with
                |AutoGenerated -> ca.Dispose()
                |_ -> ()
            |_ -> ()
/// Compute data types
and internal ComputeDataType =
    |ComputeFloat
    |ComputeFloat32
    |ComputeBool

/// Functions to query information stored in the device array
module internal ComputeDataInfo = 
    let length = 
        function
        |ComputeFloat -> sizeof<float>
        |ComputeFloat32 -> sizeof<float32>
        |ComputeBool -> sizeof<float32>

//
// GPU Types
// ---------
// These types do not and should not have implemented functionality.  They exist only to allow the F# type-checker to enforce the correctness of expressions.

/// A marker for types which reside in device memory instead of host memory.
type IGPUType = interface end

/// A bool-equivalent type which resides in device memory
type devicebool = class
    interface IGPUType
    end

/// A (double precision) floating point number which resides in device memory
type devicefloat =
    interface IGPUType
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member ( + ) (flt1 : devicefloat, flt2 : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member ( + ) (flt1 : devicefloat, flt2 : float) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member ( + ) (flt1 : float, flt2 : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member ( - ) (flt1 : devicefloat, flt2 : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member ( - ) (flt1 : devicefloat, flt2 : float) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member ( - ) (flt1 : float, flt2 : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member ( * ) (flt1 : devicefloat, flt2 : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member ( * ) (flt1 : devicefloat, flt2 : float) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member ( * ) (flt1 : float, flt2 : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member ( / ) (flt1 : devicefloat, flt2 : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member ( / ) (flt1 : devicefloat, flt2 : float) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member ( / ) (flt1 : float, flt2 : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Pow (flt1 : devicefloat, flt2 : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Pow (flt1 : devicefloat, flt2 : float) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Pow (flt1 : float, flt2 : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Sin (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Cos (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Tan (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Sinh (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Cosh (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Tanh (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Asin (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Acos (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Atan (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Sqrt (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Log (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Log10 (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member Exp (flt : devicefloat) : devicefloat = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member CompareTo (flt1 : devicefloat, flt2 : float) = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member CompareTo (flt1 : devicefloat, flt2 : devicefloat) = DeviceError.raiseNotSupportedOperation()
    /// This method exists to create correct type-checking behaviour of device expressions.  It should not be called directly.
    static member CompareTo (flt1 : float, flt2 : devicefloat) = DeviceError.raiseNotSupportedOperation()

[<AutoOpen>]
/// Basic device operators.  This module is automatically opened in F# GPU.
module GPUOperators =
    /// device capable greater than operator
    let inline (.>.) (val1 : ^a) (val2 : ^b) : devicebool =
        let compare = ((^a or ^b)  : (static member CompareTo : ^a * ^b -> devicebool) (val1, val2))
        DeviceError.raiseNotSupportedOperation()
    /// device capable greater than or equal operator
    let inline (.>=.) (val1 : 'a) (val2 : 'b) : devicebool =
        let compare = ((^a or ^b)  : (static member CompareTo : ^a * ^b -> devicebool) (val1, val2))
        DeviceError.raiseNotSupportedOperation()
    /// device capable less than operator
    let inline (.<.) (val1 : 'a) (val2 : 'b) : devicebool =
        let compare = ((^a or ^b)  : (static member CompareTo : ^a * ^b -> devicebool) (val1, val2))
        DeviceError.raiseNotSupportedOperation()
    /// device capable less than or equal operator
    let inline (.<=.) (val1 : 'a) (val2 : 'b) : devicebool =
        let compare = ((^a or ^b)  : (static member CompareTo : ^a * ^b -> devicebool) (val1, val2))
        DeviceError.raiseNotSupportedOperation()
    /// device capable equality operator
    let ( .=. ) val1 val2 : devicebool  = DeviceError.raiseNotSupportedOperation()
    /// device capable inequality operator
    let ( .<>. ) val1 val2 : devicebool  = DeviceError.raiseNotSupportedOperation()
    /// device capable conditional AND operator
    let ( .&&. ) (val1 : devicebool) (val2 : devicebool) : devicebool  = DeviceError.raiseNotSupportedOperation()
    /// device capable conditional OR operator
    let ( .||. ) (val1 : devicebool) (val2 : devicebool) : devicebool  = DeviceError.raiseNotSupportedOperation()

type internal DeviceArrayCombinations =
    |SingleItemArray of ComputeArray
    |Tuple2Array of ComputeArray*ComputeArray
    |Tuple3Array of ComputeArray*ComputeArray*ComputeArray

/// The type of immutable arrays of generic type which reside in device memory
type devicearray<'a> internal (arrays) = 
    member internal this.DeviceArrays = arrays
    internal new (devArray : ComputeArray) = new devicearray<'a>(SingleItemArray devArray)
    internal new (devArray1 : ComputeArray, devArray2 : ComputeArray) = new devicearray<'a>(Tuple2Array (devArray1, devArray2))
        
    /// Frees the device memory associated with this object
    override this.Finalize() = 
        match arrays with
        |SingleItemArray devArray -> devArray.Dispose()
        |Tuple2Array (devArray1, devArray2) ->
            devArray1.Dispose()
            devArray2.Dispose()
        |Tuple3Array (devArray1, devArray2, devArray3) ->
            devArray1.Dispose()
            devArray2.Dispose()
            devArray3.Dispose()
    interface System.IDisposable with
        member this.Dispose() = 
            match arrays with
            |SingleItemArray devArray -> devArray.Dispose()
            |Tuple2Array (devArray1, devArray2) ->
                devArray1.Dispose()
                devArray2.Dispose()
            |Tuple3Array (devArray1, devArray2, devArray3) ->
                devArray1.Dispose()
                devArray2.Dispose()
                devArray3.Dispose()

/// The type of immutable single element of generic type which reside in device memory
type deviceelement<'a> internal (devArray : ComputeArray) = 
    do match devArray.Length with
        |1 -> ()
        |_ -> raise <| System.InvalidOperationException("deviceelement must have one element")
    member internal this.DeviceArray = devArray
    /// Frees the device memory associated with this object
    override this.Finalize() = devArray.Dispose()
    interface System.IDisposable with
        member this.Dispose() = devArray.Dispose()




    

