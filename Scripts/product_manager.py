import re
import random


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

    n_has = len(has_combo)
    n_no = len(no_combo)
    p_has = n_has/float(n_no) * 100
    print('{0} total combinations'.format(n_has + n_no))
    print('{0} have combinations ({1:.2f}%)'.format(n_has, p_has))
    print('{0} don\'t have combinations'.format(n_no))

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
    main()
