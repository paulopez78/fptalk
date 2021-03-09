const compose = (fn1, fn2) => x => fn1(fn2(x));
const pipe = (...fns) => x => fns.reduce((v, f) => f(v), x);

const curry = (f, arr = []) => (...args) => 
    ( a => a.length === f.length ? 
        f(...a) : 
        curry(f, a))([...arr, ...args]);

function Add (a, b) {
    return a + b;
}

function log (message, value) {
    console.log(`${message}: ${value}`)
}

const log2 = curry(log);

const add = a => b => a + b;
const isEven = a => a % 2 === 0;

const add2 = add(2);
const add5 = add(5);

//const result = isEven(add2(2));
//const result = compose(isEven, add2, 2)
const result = 2 |> add2 |> isEven;

console.log(`RESULT PIPE: ${result}`);

const map = f => arr => arr.map(f);
const liftAdd2 = map(add2);

//var years = [3,4,5,7,0].map(add2);
var years2 = liftAdd2([3]);
var years3 = add2(3);

const add2ThenIsEven = pipe(
    log2('Before adding 2'),
    add2, 
    log2('After adding 2'),
    isEven,
    log2('After is even'),
    )

//const result =  add2ThenIsEven(2);

const level = years =>
    years >= 10 
    ? 'Rockstar' 
    : years >= 8 
    ? 'Ninja' 
    : years >= 5 
    ? 'Senior' 
    : years >= 3 
    ? 'Mid' : 
    years > 0 
    ? 'Junior' 
    : "No idea";

const promote = level =>
    level === 'Ninja'
    ? 'Rockstar' 
    : level === 'Senior'
    ? 'Ninja' 
    : level === 'Mid'
    ? 'Senior' 
    : level === 'Junior'
    ? 'Mid' 
    : level;

const years = level =>
    level === 'Rockstar'
    ? 12 
    : level === 'Ninja'
    ? 10 
    : level === 'Senior'
    ? 8
    : level === 'Mid'
    ? 4 
    : level === 'Junior'
    ? 1
    : 0;

const isSenior = years => years >= 5

test('is senior function', () => {
  expect(isSenior(10)).toBe(true);
});

test('are seniors map', () => {
  const areSeniors = [8, 5, 2, 2, 10, 0].map(isSenior);
  expect(areSeniors).toEqual([true, true, false, false, true, false]);
});

test('functional pipeline', () => {
  const areSeniors = [8, 5, 2, 2, 10, 0].map(level).map(promote).map(years).map(isSenior);
  expect(areSeniors).toEqual([true, true, false, false, true, false]);
});

test('functional pipeline simple', () => {
    const pipeline = 
    pipe(
        level,
        promote,
        years,
        isSenior
    );

    const pipeline2 = () => 
        level 
        |> promote
        |> years 
        |> isSenior;

    console.log(pipeline2(2));

    expect(pipeline(4)).toEqual(true);
    expect(pipeline2(2)).toEqual(false);
});
