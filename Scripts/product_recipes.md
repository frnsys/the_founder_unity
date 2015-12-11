
m_Name: AI.Android
names: Servo, Alfred, Handy
description: Automated labor and service
    - wage multiplier - 0.2

m_Name: AI.Gadget
names: DriveIt, Chaffeur
description: Driverless cars

m_Name: AI.Defense
names: EagleEye, DefenseWatch
description: National automated defense and threat-detection system
    - cash bonus: $100000 x product quality

m_Name: AI.Implant
names: Think4Me, WWID
description: Bad at making tough decisions? Not sure what the morally correct course

m_Name: AI.Mobile
names: Her&Him, PhoneFriend
description: AI companionship in a mobile device
    - rate of forgetting +0.05
    - synergies: mobile.wearable

m_Name: Ad.Analytics
names: HyperAd
description: Hypertargeted ad networks.
    - spending rate + 0.02
    - marketing + to all products

m_Name: Ad.Genetics
names: GeneAd
description: Genetics ad profiling network
    - spending rate + 0.05
    - marketing + to all products

m_Name: Ad.Implant
names: BrainShare
description: Thrifty consumers can rent out thoughtspace for thought-ads and save
    - spending rate + 0.08
    - marketing + to all products

m_Name: Ad.Social Network
names: PalHub, FaceSpace
description: A social networking site - people can keep up with friends, meet new
    - spending rate + 0.01
    - forgetting rate + 0.01
    - marketing + to all products
    - synergies: ad.analytics

m_Name: Ad.Virtual Reality
names: VirtuAd
description: VR ad network
    - spending rate + 0.03
    - marketing + to all products
    - synergies: ad.analytics

m_Name: Analytics.Gadget
names: SmartHome
description: Home automation devices

m_Name: Analytics.Credit
names: "Trade2000"
description: High-frequency trading algorithms
    - economic stability  - 0.05

m_Name: Analytics.Defense
names: PreCrime
description: PreCrime systems
    - cash bonus $100000 x product quality

m_Name: Analytics.E-Commerce
names: BuyMe
description: Personalized shopping platform
    - happiness + 1

m_Name: Analytics.Entertainment
names: Entertainalytics
description: Personalized streaming media services.
    - forgetting rate + 0.02
    - synergies: socialnetwork.entertainment, celebrity.entertainment, ecommerce.entertainment

m_Name: Analytics.Genetics
names: TheraGene
description: Gene therapy treatments
    - synergies: drug.genetics

m_Name: Analytics.Implant
names: ThoughtChip
description: The next level of quantified self - quantified psychological tracking

m_Name: Analytics.Social Network
names: UScore, WhatImWorth
description: A finely-detailed metric of your intrinsic social value, based on your

m_Name: Analytics.Wearable
names: Wearalytics, FitNis
description: Fitness trackers and quantified self devices
    - unlocks worker details

m_Name: Android.Celebrity
names: RoboPopStar
description: Robo pop star
    - forgetting rate + 0.05
    - synergies: celebrity.entertainment

m_Name: Android.Credit
names: BuyBot
description: What do we even need *people* to buy our products for anyways? We can
    - spending rate + 0.2

m_Name: Android.Defense
names: RoboWarrior
description: Robotic soldiers and law enforcement
    - cash bonus $100000 x product quality

m_Name: Gadget.Space
names: Sats
description: Satellites
    - synergies: defense.space

m_Name: Celebrity.Entertainment
names: CheesyMovieName
description: Blockbuster film
    - forgetting rate + 0.01
    - synergies: ecommercee.entertainment, entertainment.social network, celebrity.genetics, celebrity.synthetic organism, celebrity.virtual reality, robo celebrity, analytics.entertainment

m_Name: Celebrity.Genetics
names: CelebGenes
description: Licensed genetic sequences from your favorite celebrities, ready to
    - synergies: celebrity.entertainment, ecommerce.genetics

m_Name: Celebrity.Synthetic Organism
names: DNACelebs
description: Vat-grown celebrities - perfect down to their DNA.
    - forgetting rate + 0.04
    - synergies: celebrity.entertainment

m_Name: Celebrity.Virtual Reality
names: Hayuke Minu, HoloPac
description: Perfectly-personalized holographic celebrities for every demographic
    - forgetting rate + 0.03
    - synergies: VR headset, VR social network, cognitive.vr, celebrity.entertainment

m_Name: Cognitive.Drug
names: Xany
description: Cognitive emotional control and enhancement drugs
    - happiness + 5
    - productivity + 5
    - synergies: drug.implant

m_Name: Cognitive.Entertainment
names: TotalMemory
description: Simulated memory experiences
    - forgetting rate + 0.06

m_Name: Cognitive.Social Network
names: HiveMind, GroupThink, MindMeld
description: A social cognitive experience - merge thoughts and become a collective
    - forgetting rate + 0.02
    - synergies: cognitive.vr

m_Name: Cognitive.Virtual Reality
names: MindWorld
description: A virtual world for your mind
    - forgetting rate + 0.05
    - synergies: vr.celebrity, vr.social network, cognitive.social network

m_Name: Credit.E-Commerce
names: UberCard, Pisa, USA Express
description: People want to buy stuff, but can't because they don't have money.
    - spending rate + 0.1
    - synergies: credit.implant

m_Name: Credit.Implant
names: LoanNeural
description: Keep detailed track of risky loaners with these neural loan monitoring
    - economic stability + 0.05
    - synergies: credit.ecommerce

m_Name: Credit.Mobile Device
names: PayBack, Tipper, CashPal, Cube
description: We can empower many small businesses and inviduals that need to process
    - spending rate + 0.06

m_Name: Credit.Social Network
names: SpareChange, LendAHand, GimmeMoney
description: A microfinancing platform makes sure credit is available for everyone.
    - spending rate + 0.04

m_Name: Default
names:
description:

m_Name: Defense.Drug
names: Furinol
description: Enhancement-performing and aggression drugs for soldiers.
    - cash bonus $100000
    - synergies. drug.implant

m_Name: Defense.Entertainment
names: WarTour
description: The thrill of cinema not enough? Tour conflict zones under the safety
    - forgetting rate + 0.04

m_Name: Defense.Genetics
names: GeneKill
description: Bioweapons with genetic-targeting systems.
    - cash bonus $100000

m_Name: Defense.Space
names: SpaceStrike, Zeus
description: Orbital weaponry
    - cash bonus $100000
    - synergies: appliance.space

m_Name: Defense.Wearable
names: exMan, eXo, Skeltor
description: Exoskeletons
    - cash bonus $100000

m_Name: Drug.Entertainment
names: Happi
description: Recreational drugs
    - forgetting rate + 0.05
    - happiness + 5
    - productivity - 2
    - synergies: drug.implant

m_Name: Drug.Genetics
names: GeneTherapy
description: Genetic therapy treatments
    - happiness + 5
    - synergies:  analytics.genetics

m_Name: Drug.Implant
names: Moody
description: Mood pacemakers
    - happiness + 10
    - synergies: drug.entertainment, drug.cognitive, defense.drug

m_Name: E-Commerce.Entertainment
names: TuneStore, BuyMeDia
description: DRM'd entertainment products
    - forgetting rate + 0.02
    - synergies: entertainment.mobile, entertainment.celebrity, analytics.entertainment

m_Name: E-Commerce.Genetics
names: BabyStop
description: Online marketplace for designer babies and other genetics-based products.
    - synergies: celebrity.genetics

m_Name: E-Commerce.Mobile Device
names: AppShop, SoftStore
description: A marketplace where aspiring companies can sell their software.
    - synergies: mobile.wearable, entertainment.mobile, appliance.mobile

m_Name: E-Commerce.Social Network
names: AirShare, Suber, HireMe
description: A social network where people can "share" goods and services by getting

m_Name: Entertainment.Mobile
names: yPad
description: Tablets
    - synergies: ecommerce.entertainment, ecommerce.mobile

m_Name: Entertainment.Social Network
names: Flixspot, InstaVids, StreamIt
description: TV is dead - people don't want to have to leave their computers to
    - forgetting rate + 0.02
    - synergies: entertainment + celebrity, analytics.entertainment

m_Name: Entertainment.Space
names: galactica
description: Space trips
    - forgetting rate + 0.03

m_Name: Genetics.Social Network
names: Genee, DTFDNA
description: Genetics-based dating social network
    - synergies: analytics.genetics, ad.genetics

m_Name: Implant.Mobile
names: yOptics
description: Retinal displays

m_Name: Mobile.Social Network
names: LetsChat, SupApp, FaceTalk, Blabber
description: People have phones and use social networks. Seems like a natural combination.
    - happiness + 4
    - forgetting rate + 0.02
    - synergies: ad social network, analytics ad, appliance.mobile

m_Name: Mobile.Wearable
names: yWatch
description: Smartwatches
    - synergies: ecommerce+ mobile device

m_Name: Social Network.Virtual Reality
names: Matrix, OZ, The Network
description: Our virtual reality social network will give us unprecedented direct
    - happiness + 8
    - forgetting rate + 0.06
    - synergies: vr headset, vr celebrity, ad.vr

m_Name: Virtual Reality.Wearable
names: VRT, RetinalVR
description: VR headset
    - synergies: vr social network, vr celebrity

---


TO DO:
- synergies for these
- effects for these
- stats for these

(appliance/sensors)

mobile + appliance
    smartphone
