let add1 x = x + 1 // 型シグネチャ: int -> int
printfn "add1 10 = %d" (add1 10)

let add x y = x + y // 型シグネチャ: int -> int -> int
printfn "add 10 20 = %d" (add 10 20)

let squarePlusOne x = let square = x * x in square + 1 //  型シグネチャ: int -> int
printfn "squarePlusOne 10 = %d" (squarePlusOne 10)

// areEqual : `a -> `a -> bool
// アポストロフィがあると、ジェネリック型を表す
let areEqual x y = x = y
printfn "areEqual 10 10 = %b" (areEqual 10 10)


// type AppleVariety = {
//     Name: string
//     Color: string
// }

// type BananaVariety = {
//     Name: string
//     Color: string
// }

// type CherryVariety = {
//     Name: string
//     Color: string
// }

type AppleVariety = 
| GoldenDelicious
| GrannySmith

type BananaVariety =
| Yellow
| Red

type CherryVariety =
| Sweet
| Sour

// レコード型
// レコード型は、複数のフィールドを持つことができる
// And型
type FruitSalad = {
    Apple: AppleVariety
    Banana: BananaVariety
    Cherryies: CherryVariety
}

// Or型
// 縦棒はそれぞれの選択肢を区切っている
// このような選択型を判別共用体という
type FruitSnack = 
| Apple of AppleVariety
| Banana of BananaVariety
| Cherries of CherryVariety

// OR型の使い方
// match式を使う

let myApple = GoldenDelicious
let myBanana = Red
let myCherries = Sweet

// バナナの種類に応じて、説明文を返す関数
let describeBanana (banana: BananaVariety) =
    match banana with
    | Yellow -> "Yellow Banana"
    | Red -> "Red Banana"

printfn "describeBanana  = %s" (describeBanana myBanana)
