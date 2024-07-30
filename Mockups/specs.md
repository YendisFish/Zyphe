# Variables
```rust
let x = new MyStruct(); // mutable variable
const y = new MyStruct(); // immutable variable
```

# References
```cs
let x = 5;

ref someRef = &x; //automatically makes reference since its a reference type
ref someOtherRef = &someRef; //error, only shallow references are allowed
ref anotherRef = someRef; // copies over reference

/*
The reason this is implemented this way is to prevent people from just spamming references where they aren't needed
*/

let y = someOtherRef; //this will dereference until it gets to a value type
let z = &someOtherRef; //error, you are trying to pass a reference type into a value type

myFunction(&someOtherRef /* in order to avoid automatic dereferencing we must pass this as a reference */);
```

```rs
ref emptyBuffer: int = [100]; // allocates an empty buffer with a length of 100
```

# Structs
```cs
struct MyStruct {
    let x: string 
    let y: int 
    let z: bool {
        get;
        set => z = value;
    }

    MyStruct(default);

    /*
    MyStruct(default); converts to this:

    MyStruct(x: string, y: int, z: bool) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    */

    void DoSomething(let newX: string) {
        x = newX;
    }

    int addAndGetY(let toAdd: int) {
        y = y + toAdd;
        return y;
    }
    
    //you can do conversions with the this keyword
    string this {
        get {
            return this.x;    
        }
        
        set {
            this.x = value;
        }     
    }
}
```

# Allocating on the heap
```cs
ref myClass = new MyStruct();
```

# Struct Variable Type

As an anonymous type
```rs
//I chose not to use "let" here to specify that this type/variable is compiler generated
struct myStruct = {
  let x: int = 5,
  let y: string = "foo",
  ref z: char = { 'a', 'b', 'c' } // arrays are automatically allocated to the heap with refs
};

let someObj: myStruct = new myStruct() {
  x = 5,
  y = "foo",
  z = myStruct.z
};
```

As a container for type information
```rs
struct myStruct
{
  let x: int;
}

let myObj = new myStruct();

struct typeContainer = myObj; //compiler recognizes this is a "struct" variable and inserts the type information

let obj2: typeContainer = new typeContainer();
obj2.x = 100;
```

# Errors

```ts
let x = try {
    throw new Error();
} : (let ex: Exception) => {
    return 5;
}

let y: int = try MyFunc() : SomeFunction();
int SomeFunction(Exception ex);
int MyFunc();

let z: int = try {} : default /* (Exception ex) => int.default */;
```

```ts
try {
    
} catch(let exception: Exception) {
    
}

```

# Indexers

```cs
struct SomeThing {
    private ref ptr: char;

    char this[let index: int] {
        get {
            return ptr[index];
        }

        set {
            ptr[index] = value;
        }
    }
}
```

# From block

```c++
let x = 5;
extern Thread runSomeThreadedOperation(let num: int);

// pause the current thread until x is equal to 10
from(runSomeThreadedOperation(&x)) {
    x == 10 => io.conout("x is 10!");
    _ => break;
}

// same as
ref someThreadInfo = runSomeThreadedOperation(&x);
while(!someThreadInfo.HasExited) {
    switch(x) {
        10 => io.conout("x is 10!");
        _ => break;
    }
}
```

# Switch Statements
```ts
let x = 5;
switch(x) {
    4 => throw new Error(),
    5 => io.conout("x is 5!")
    _ => break;
}
```

# Generics
```cs
struct MyStruct<T> {
    let x: T;

    T GetX() => x;
}

// you can also use constraints

struct A {
    let x: int;.
}

struct B {
    let y: string;
}

// specify that T can be of type A or B
struct MyStruct<T : A, B> {
    ref variable: T;
}
```

# Function Pointers
```cs
int MyFunc(let a: int) {
    return a + 1;
}

ref myFunc: delegate<int, int> = &MyFunc; // delegate<return type, parameter...>

let output = myFunc(5);
```

```cs
ref myFunc: delegate<int> = (ref delegate<int>)0x0238479231494; //address of some function in memory
io.conout(myFunc());
```

# External functions

CPP function:
```c++
int FunctionFromCpp(char* string) {
    int len = 0;
    //calculate string length
    return len;
}
```

Zyphe code:
```rs
extern int FunctionFromCpp(ref string: char);

let len: int = FunctionFromCpp( {'a', 'b', 'c'} ); // on initialization refs do not need &, this is why & is missing from this ref assignment
io.conout(len);
```

# Fixed Buffers
```rs
const x: char = {'a', 'b', 'c'}; // allocated on stack
const y: char = [5]; // compiler recognizes the const type and allocates on the stack
```

# Ref Consts
```rs
ref const y = {'a', 'b', 'c'}; // pretty simple, this is immutable and allocated on the heap
```

# Arrays

Arrays are special and are processed differently from other ref types. They
are treated as 1 pointer to a block of their respective type. So dont be confused
and think this syntax creates references to references.

```rs
let x: char[] = new char[100]; // error, arrays allocated on the stack must be of type "const"
ref y: char[] = new char[100]; // works

ref z: char = &y[0]; // you can convert an array to a normal ref like so
ref.UnsafeLengthSetter(&z, 100);
```

2d Arrays
```rs
ref x: char[][] = new char[100][100];
ref y: char[] = x[5]; // automatically passed as a reference since it is an array
ref z: char = &y[0];

let a: char = z;
```

# Circular References
Since Zyphe utilizes ARC for memory, it makes circular references
hard to deal with. For this, the ``using`` keyword exists. This allows
you to create a reference that is not tracked by ARC, but is the same
as a ``ref`` in every other way.

```csharp
struct LinkedList {
    ref head: Node<int> { get; set; }
}

struct Node<T> {
    using prev: Node<T> { get; set; }
    let value: T { get; set; }
    ref next: Node<T> { get; set; }
}
```