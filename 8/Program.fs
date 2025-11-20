// F#で関数を「もの」として扱う
let plus3 x = x + 3
let time2 x = x * 2

// ２つのsquareの定義は本質的に同じ
// let square = (fun x -> x * x)
let square x = x * x
let addThree = plus3

// 関数をリストとしてまとめる
let listOfFunctions = [ addThree; time2; square ]

// リストをループして、各関数を順番に評価する
for fn in listOfFunctions do
    let result = fn 100 // 関数を呼び出す
    printfn "If 100 is the input, the output is %i" result

// 入力としての関数
let evalWith5ThenAdd2 fn = fn (5) + 2

// let add1 x = x + 1
// let result = evalWith5ThenAdd2 add1
// printfn "evalWith5ThenAdd2 output is %i" result

// 出力としての関数
let addGenerator numberToAdd =
    // ラムダを返す
    fun x -> numberToAdd + x

// 入力「x」に対して「1」を加算する関数を生成
let add1 = addGenerator 1
let add1result = add1 2
printfn "add1result is %i" add1result
// 入力「x」に対して「100」を加算する関数を生成
let add100 = addGenerator 100
let add100result = add100 2
printfn "add100result is %i" add100result

// カリー化
// 多パラメーターの関数でも、1パラメーターの関数を連なったものに変換する
// let add x y = x + y

// int -> (int -> int)
// let addGenerator x = fun y -> x + y

// 部分適用
