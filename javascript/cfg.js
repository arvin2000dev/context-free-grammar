function firstSetRaHesabKon(grammar) {
    let firstSet = {};
    let epsilon = 'Є';

    for (let nonTerminal in grammar) {
        firstSet[nonTerminal] = new Set();
    }

    let hasChanged = true;
    while (hasChanged) {
        hasChanged = false;
        for (let nonTerminal in grammar) {
            let productions = grammar[nonTerminal];
            let oldSize = firstSet[nonTerminal].size;
            for (let production of productions) {
                let symbols = production.split('');
                let allSymbolsCanProduceEpsilon = true;
                for (let symbol of symbols) {
                    if (grammar[symbol]) {
                        for (let s of firstSet[symbol]) {
                            if (s !== epsilon) {
                                firstSet[nonTerminal].add(s);
                            }
                        }
                        if (!firstSet[symbol].has(epsilon)) {
                            allSymbolsCanProduceEpsilon = false;
                            break;
                        }
                    } else {
                        firstSet[nonTerminal].add(symbol);
                        allSymbolsCanProduceEpsilon = false;
                        break;
                    }
                }
                if (allSymbolsCanProduceEpsilon) {
                    firstSet[nonTerminal].add(epsilon);
                }
            }
            if (firstSet[nonTerminal].size !== oldSize) {
                hasChanged = true;
            }
        }
    }

    return firstSet;
}

let grammar1 = {
    'S': ['aBDh'],
    'B': ['cC'],
    'C': ['bC', 'Є'],
    'D': ['EF'],
    'E': ['g', 'Є'],
    'F': ['f', 'Є']
};

let grammar2 = {
    'S': ['ACB', 'Cbb', 'Ba'],
    'A': ['da', 'BC'],
    'B': ['g', 'Є'],
    'C': ['h', 'Є']
};

let grammar3 = {
    'E': ['TE’'],
    'E’': ['+TE’', 'Є'],
    'T': ['FT’'],
    'T’': ['*FT’', 'Є'],
    'F': ['(E)', 'i']
};

function followSetRaHesabKon(grammar, firstSet) {
    let followSet = {};
    let epsilon = 'Є';
    let startSymbol = Object.keys(grammar)[0];

    for (let nonTerminal in grammar) {
        followSet[nonTerminal] = new Set();
    }

    followSet[startSymbol].add('$');

    let hasChanged = true;
    while (hasChanged) {
        hasChanged = false;
        for (let nonTerminal in grammar) {
            let productions = grammar[nonTerminal];
            for (let production of productions) {
                let symbols = production.split('');
                for (let i = 0; i < symbols.length; i++) {
                    if (grammar[symbols[i]]) { 
                        let oldSize = followSet[symbols[i]].size;
                        if (i < symbols.length - 1) { 
                            if (grammar[symbols[i + 1]]) { 
                                for (let s of firstSet[symbols[i + 1]]) {
                                    if (s !== epsilon) {
                                        followSet[symbols[i]].add(s);
                                    }
                                }
                                if (firstSet[symbols[i + 1]].has(epsilon)) {
                                    for (let j = i + 2; j < symbols.length; j++) {
                                        if (grammar[symbols[j]]) {
                                            for (let s of firstSet[symbols[j]]) {
                                                if (s !== epsilon) {
                                                    followSet[symbols[i]].add(s);
                                                }
                                            }
                                            if (!firstSet[symbols[j]].has(epsilon)) {
                                                break;
                                            }
                                        } else {
                                            followSet[symbols[i]].add(symbols[j]);
                                            break;
                                        }
                                    }
                                }
                            } else {
                                followSet[symbols[i]].add(symbols[i + 1]);
                            }
                        }
                        if (i === symbols.length - 1 || symbols.slice(i + 1).every(symbol => firstSet[symbol]?.has(epsilon))) {
                            for (let s of followSet[nonTerminal]) {
                                followSet[symbols[i]].add(s);
                            }
                        }
                        if (followSet[symbols[i]].size !== oldSize) {
                            hasChanged = true;
                        }
                    }
                }
            }
        }
    }

    return followSet;
}

let exampleGrammar = {
    'S': ['AB'],
    'A': ['Sx', 'x', 'Є'],
    'B': ['yB', 'y'],
    'C': ['ACA', 'z']
};

console.log("first: ", firstSetRaHesabKon(exampleGrammar));
console.log("follow: ", followSetRaHesabKon(exampleGrammar, firstSetRaHesabKon(exampleGrammar)));

console.log("grammar1: ", followSetRaHesabKon(grammar1, firstSetRaHesabKon(grammar1)));
console.log("grammar2: ", followSetRaHesabKon(grammar2, firstSetRaHesabKon(grammar2)));
console.log("grammar3: ", followSetRaHesabKon(grammar3, firstSetRaHesabKon(grammar3)));
