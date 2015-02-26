import re
import sys
import random
import operator


class Combo():
    def __init__(self, combo, raw_line):
        self.combo = combo
        self.raw = raw_line.replace('\n', '')


def load():
    combo_re = re.compile(r'(.+)\s\+\s(.+)=>')
    result_re = re.compile(r'.+=>(.+)')

    has_combo = []
    no_combo = []

    with open('products.md', 'r') as f:
        for line in f.readlines():
            ps = combo_re.match(line)
            p1 = ps.group(1).strip()
            p2 = ps.group(2).strip()

            combo = Combo((p1, p2), line)
            match = result_re.match(line)

            if match is not None and match.group(1).strip():
                combo.result = match.group(1).strip()
                has_combo.append(combo)
            else:
                no_combo.append(combo)

    return has_combo, no_combo


def main():
    has_combo, no_combo = load()

    types = {}
    for combo in no_combo:
        for type in combo.combo:
            if type not in types:
                types[type] = 0
            types[type] += 1

    n_has = len(has_combo)
    n_no = len(no_combo)
    n_types = len(types)
    p_has = n_has/float(n_no) * 100
    print('{0} product types'.format(n_types))
    print('{0} total combinations'.format(n_has + n_no))
    print('{0} have combinations ({1:.2f}%)'.format(n_has, p_has))
    print('{0} don\'t have combinations'.format(n_no))

    print('Usage amounts (higher is better):')
    sorted_types = sorted(types.items(), key=operator.itemgetter(1))
    for type, count in sorted_types:
        print('  {0}:{1:.2f}% ({2})'.format(type, (n_types - count - 1)/float(n_types - 1) * 100, n_types - count - 1))

    print('-'*30)
    print('Selecting random incomplete combo...')
    combo = random.choice(no_combo)
    print(combo.combo)
    result = raw_input('Define a result: ')

    if result:
        with open('products.md', 'r') as f:
            doc = f.read()

        doc = doc.replace(combo.raw, '{0} {1}'.format(combo.raw, result))

        with open('products.md', 'w') as f:
            f.write(doc)


if __name__ == '__main__':
    try:
        main()
    except KeyboardInterrupt:
        print()
        sys.exit(0)
