import itertools

product_types = [
    # Information
    'social network',
    'e-commerce',
    'ai',
    'analytics',
    'ad',

    # Entertainment
    'virtual reality',
    'celebrity',
    'entertainment',

    # Finance
    'security (financial)',
    'credit',

    # Hardware
    'mobile device',
    'wearable',
    'iot',
    'transportation',
    'nanorobots',
    'drones',
    'space',
    'android',

    # Defense
    'weapon',
    'tactical gear',
    'security (national)',

    # Biotech
    'drug',
    'synthetic organism',
    'genetics',
    'cognitive',
    'bioenhancement',
    'implant',  # requires hardware as well
]

combos = list(itertools.combinations(product_types, 2))

print('{0} product types'.format(len(product_types)))
print('{0} combos'.format(len(combos)))
print('---')

length = 44
for combo in combos:
    out = '{0} + {1}'.format(combo[0], combo[1])
    print('{0}{1}=> '.format(out, ' ' * (length - len(out))))