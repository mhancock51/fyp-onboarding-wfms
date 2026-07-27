"use client"

import { useEffect, useState } from "react"
import {
  ArrowRight,
  CheckCircle2,
  Clock,
  FileText,
  LayoutDashboard,
  Menu,
  Shield,
  Star,
  Users,
  X,
} from "lucide-react"
import { Button } from "@/components/ui/button"

const APP_URL = import.meta.env.VITE_APP_URL || "http://localhost:5173"
const BACKEND_API_URL = import.meta.env.VITE_BACKEND_API_URL || "http://localhost:5000/api"

const navigation = [
  { name: "Features", href: "#features" },
  { name: "Pricing", href: "#pricing" },
  { name: "How It Works", href: "#how-it-works" },
  { name: "FAQ", href: "#faq" },
]

const features = [
  {
    icon: LayoutDashboard,
    title: "Visual Workflow Builder",
    description:
      "Drag-and-drop interface to design onboarding workflows tailored to every role, department, and location.",
  },
  {
    icon: Clock,
    title: "Automated Task Assignment",
    description:
      "Auto-assign checklist items, document requests, and training modules the moment a new hire is added.",
  },
  {
    icon: Users,
    title: "Role-Based Dashboards",
    description:
      "Dedicated views for HR, managers, IT, and new hires — everyone sees exactly what they need to do.",
  },
  {
    icon: FileText,
    title: "Document Management",
    description:
      "Collect signed offer letters, tax forms, and policy acknowledgements — all stored securely per employee.",
  },
  {
    icon: Shield,
    title: "Compliance & Audit Trails",
    description:
      "Every step is logged with timestamps. Prove compliance with SOC 2, ISO 27001, and internal policies.",
  },
  {
    icon: CheckCircle2,
    title: "Progress Tracking",
    description:
      "Real-time dashboards show onboarding completion rates, bottlenecks, and time-to-productivity metrics.",
  },
]

type PricingTier = {
  id: string
  name: string
  displayName: string
  description: string
  unitAmount: number | null
  currency: string
  interval: string
  priceDisplay: string
  highlighted: boolean
  ctaText: string
  features: string[]
}

// Static fallback tiers used only when the API is completely unreachable.
// In normal operation, pricing is fetched dynamically from the backend →
// Stripe so prices and features are always up to date.
const fallbackTiers: PricingTier[] = [
  {
    id: "tier-1-subscription",
    name: "tier-1-subscription",
    displayName: "Starter",
    description: "Perfect for small teams getting started with structured onboarding.",
    unitAmount: 4900,
    currency: "USD",
    interval: "month",
    priceDisplay: "$49/month",
    highlighted: false,
    ctaText: "Start Free Trial",
    features: [
      "Up to 5 workflow templates",
      "10 task templates",
      "5 active onboardings",
      "Basic document storage (100 MB)",
      "Email support",
    ],
  },
  {
    id: "tier-2-subscription",
    name: "tier-2-subscription",
    displayName: "Professional",
    description: "For growing companies that need advanced workflows and integrations.",
    unitAmount: 14900,
    currency: "USD",
    interval: "month",
    priceDisplay: "$149/month",
    highlighted: true,
    ctaText: "Start Free Trial",
    features: [
      "Unlimited workflow templates",
      "Unlimited task templates",
      "50 active onboardings",
      "1 GB document storage",
      "Custom branding",
      "API access & webhooks",
      "Priority support",
    ],
  },
  {
    id: "tier-3-subscription",
    name: "tier-3-subscription",
    displayName: "Enterprise",
    description: "For large organizations with complex, multi-department onboarding needs.",
    unitAmount: null,
    currency: "",
    interval: "",
    priceDisplay: "Custom",
    highlighted: false,
    ctaText: "Contact Sales",
    features: [
      "Everything in Professional",
      "Unlimited active onboardings",
      "Unlimited document storage",
      "SSO / SAML / OAuth",
      "Dedicated tenant isolation",
      "Custom integrations",
      "Dedicated account manager",
    ],
  },
]

const steps = [
  {
    number: "01",
    title: "Sign Up",
    description: "Create your account in 30 seconds. No credit card required for the trial.",
  },
  {
    number: "02",
    title: "Build Workflows",
    description: "Design onboarding templates with our visual builder or start from a pre-built template.",
  },
  {
    number: "03",
    title: "Add New Hires",
    description: "Enter a new hire's details and assign them to the right workflow with one click.",
  },
  {
    number: "04",
    title: "Track & Optimize",
    description: "Watch progress in real time and use analytics to improve your onboarding process.",
  },
]

const testimonials = [
  {
    quote:
      "OnboardFlow cut our time-to-productivity by 40%. New hires now have a clear, structured path from day one.",
    author: "Sarah Chen",
    role: "VP of People, TechScale Inc.",
  },
  {
    quote:
      "The audit trail alone paid for itself. Our SOC 2 auditor was impressed with how clean our onboarding records are.",
    author: "Marcus Rivera",
    role: "CTO, CloudPeak Solutions",
  },
  {
    quote:
      "We onboarded 200 people last quarter with zero missed steps. The role-based dashboards are a game-changer.",
    author: "Priya Patel",
    role: "HR Director, Nexus Group",
  },
]

const faqs = [
  {
    q: "How long does it take to set up?",
    a: "Most teams are up and running within an hour. We provide pre-built workflow templates for common roles so you don't have to start from scratch.",
  },
  {
    q: "Can I integrate with my HRIS?",
    a: "Yes — Professional and Enterprise plans include API access and webhooks. We also offer native integrations with popular HRIS platforms on the Enterprise tier.",
  },
  {
    q: "Is my data secure?",
    a: "Absolutely. We use AES-256 encryption at rest, TLS 1.3 in transit, and Enterprise plans get dedicated tenant database isolation. We're SOC 2 Type II certified.",
  },
  {
    q: "Can I cancel anytime?",
    a: "Yes. There are no long-term contracts on Starter and Professional plans. You can cancel at any time and export your data.",
  },
  {
    q: "Do you offer a free trial?",
    a: "Yes! Every plan comes with a 14-day free trial. No credit card required to start.",
  },
]

export function LandingPage() {
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false)
  const [tiers, setTiers] = useState<PricingTier[]>(fallbackTiers)
  const [tiersLoading, setTiersLoading] = useState(true)

  useEffect(() => {
    let cancelled = false
    async function fetchPricing() {
      try {
        const res = await fetch(`${BACKEND_API_URL}/pricing/tiers`)
        if (!res.ok) throw new Error(`HTTP ${res.status}`)
        const json = await res.json()
        if (!cancelled && json.success && Array.isArray(json.data)) {
          setTiers(json.data as PricingTier[])
        }
      } catch (err) {
        console.warn("Failed to fetch pricing from API, using fallback tiers:", err)
        // keep fallback tiers already in state
      } finally {
        if (!cancelled) setTiersLoading(false)
      }
    }
    fetchPricing()
    return () => { cancelled = true }
  }, [])

  return (
    <div className="flex min-h-screen flex-col bg-white">
      {/* Header / Nav */}
      <header className="sticky top-0 z-50 border-b bg-white/95 backdrop-blur supports-[backdrop-filter]:bg-white/60">
        <nav className="container flex h-16 items-center justify-between" aria-label="Global">
          <div className="flex items-center gap-2">
            <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary">
              <CheckCircle2 className="h-5 w-5 text-primary-foreground" />
            </div>
            <span className="text-xl font-bold tracking-tight text-foreground">
              OnboardFlow
            </span>
          </div>

          {/* Desktop nav */}
          <div className="hidden gap-6 md:flex">
            {navigation.map((item) => (
              <a
                key={item.name}
                href={item.href}
                className="text-sm font-medium text-muted-foreground transition-colors hover:text-foreground"
              >
                {item.name}
              </a>
            ))}
          </div>

          <div className="hidden items-center gap-3 md:flex">
            <Button variant="ghost" size="sm" asChild>
              <a href={`${APP_URL}/login`}>Sign In</a>
            </Button>
            <Button size="sm" asChild>
              <a href="/get-started">Get Started</a>
            </Button>
          </div>

          {/* Mobile menu button */}
          <button
            className="inline-flex items-center justify-center rounded-md p-2 text-muted-foreground md:hidden"
            onClick={() => setMobileMenuOpen(true)}
          >
            <Menu className="h-6 w-6" />
          </button>
        </nav>

        {/* Mobile menu */}
        {mobileMenuOpen && (
          <div className="fixed inset-0 z-50 md:hidden">
            <div className="fixed inset-0 bg-black/40" onClick={() => setMobileMenuOpen(false)} />
            <div className="fixed right-0 top-0 h-full w-64 bg-white p-6 shadow-xl animate-fade-in">
              <div className="flex items-center justify-between mb-8">
                <span className="text-lg font-bold">OnboardFlow</span>
                <button onClick={() => setMobileMenuOpen(false)}>
                  <X className="h-5 w-5" />
                </button>
              </div>
              <div className="flex flex-col gap-4">
                {navigation.map((item) => (
                  <a
                    key={item.name}
                    href={item.href}
                    className="text-sm font-medium text-muted-foreground"
                    onClick={() => setMobileMenuOpen(false)}
                  >
                    {item.name}
                  </a>
                ))}
                <hr />
                <Button variant="outline" size="sm" asChild>
                  <a href={`${APP_URL}/login`}>Sign In</a>
                </Button>
                <Button size="sm" asChild>
                  <a href="/get-started">Get Started</a>
                </Button>
              </div>
            </div>
          </div>
        )}
      </header>

      <main>
        {/* Hero */}
        <section className="relative overflow-hidden">
          <div className="absolute inset-0 bg-gradient-to-br from-primary/5 via-transparent to-primary/5" />
          <div className="container relative pt-20 pb-16 sm:pt-28 sm:pb-20 text-center">
            <div className="mx-auto inline-flex items-center gap-2 rounded-full border bg-muted/50 px-4 py-1.5 text-sm text-muted-foreground mb-8 animate-fade-in">
              <Star className="h-3.5 w-3.5 text-amber-500 fill-amber-500" />
              Trusted by 500+ companies worldwide
            </div>
            <h1 className="mx-auto max-w-4xl text-4xl font-extrabold tracking-tight text-foreground sm:text-5xl lg:text-6xl animate-fade-in-up">
              Employee Onboarding{" "}
              <span className="text-primary">That Actually Works</span>
            </h1>
            <p className="mx-auto mt-6 max-w-2xl text-lg text-muted-foreground animate-fade-in-up">
              Automate your onboarding workflows, eliminate manual checklists, and give every new
              hire a seamless first day — all from one platform.
            </p>
            <div className="mt-10 flex flex-col items-center gap-4 sm:flex-row sm:justify-center animate-fade-in-up">
              <Button size="xl" asChild>
                <a href="/get-started">
                  Start Free Trial
                  <ArrowRight className="ml-1 h-4 w-4" />
                </a>
              </Button>
              <Button variant="outline" size="xl" asChild>
                <a href="#features">See How It Works</a>
              </Button>
            </div>
            <p className="mt-4 text-sm text-muted-foreground">
              No credit card required · 14-day free trial
            </p>
          </div>
        </section>

        {/* Social proof bar */}
        <section className="border-y bg-muted/30 py-8">
          <div className="container">
            <p className="text-center text-sm font-medium text-muted-foreground mb-6">
              TRUSTED BY TEAMS AT
            </p>
            <div className="flex flex-wrap items-center justify-center gap-x-12 gap-y-4">
              {["TechScale", "CloudPeak", "Nexus Group", "StartupXYZ", "DataBridge", "FinCo"].map(
                (company) => (
                  <span key={company} className="text-lg font-bold text-muted-foreground/50">
                    {company}
                  </span>
                )
              )}
            </div>
          </div>
        </section>

        {/* Features */}
        <section id="features" className="py-20 sm:py-28">
          <div className="container">
            <div className="mx-auto max-w-2xl text-center mb-16">
              <h2 className="text-3xl font-bold tracking-tight sm:text-4xl">
                Everything You Need for Smooth Onboarding
              </h2>
              <p className="mt-4 text-muted-foreground">
                From offer letter to first month review — manage every step in one place.
              </p>
            </div>
            <div className="grid gap-8 sm:grid-cols-2 lg:grid-cols-3">
              {features.map((feature) => (
                <div
                  key={feature.title}
                  className="group rounded-xl border bg-card p-6 transition-shadow hover:shadow-lg"
                >
                  <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10">
                    <feature.icon className="h-6 w-6 text-primary" />
                  </div>
                  <h3 className="mb-2 text-lg font-semibold">{feature.title}</h3>
                  <p className="text-sm text-muted-foreground leading-relaxed">
                    {feature.description}
                  </p>
                </div>
              ))}
            </div>
          </div>
        </section>

        {/* How It Works */}
        <section id="how-it-works" className="bg-muted/30 py-20 sm:py-28">
          <div className="container">
            <div className="mx-auto max-w-2xl text-center mb-16">
              <h2 className="text-3xl font-bold tracking-tight sm:text-4xl">
                Get Started in Minutes
              </h2>
              <p className="mt-4 text-muted-foreground">
                Four simple steps to transform your onboarding process.
              </p>
            </div>
            <div className="grid gap-8 sm:grid-cols-2 lg:grid-cols-4">
              {steps.map((step) => (
                <div key={step.number} className="text-center">
                  <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-2xl bg-primary text-2xl font-bold text-primary-foreground">
                    {step.number}
                  </div>
                  <h3 className="mb-2 text-lg font-semibold">{step.title}</h3>
                  <p className="text-sm text-muted-foreground">{step.description}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        {/* Testimonials */}
        <section className="py-20 sm:py-28">
          <div className="container">
            <div className="mx-auto max-w-2xl text-center mb-16">
              <h2 className="text-3xl font-bold tracking-tight sm:text-4xl">
                Loved by HR Teams
              </h2>
              <p className="mt-4 text-muted-foreground">
                Here's what our customers are saying.
              </p>
            </div>
            <div className="grid gap-8 md:grid-cols-3">
              {testimonials.map((t) => (
                <div key={t.author} className="rounded-xl border bg-card p-6">
                  <div className="flex gap-1 mb-4">
                    {[...Array(5)].map((_, i) => (
                      <Star key={i} className="h-4 w-4 text-amber-500 fill-amber-500" />
                    ))}
                  </div>
                  <blockquote className="text-sm text-muted-foreground leading-relaxed mb-4">
                    &ldquo;{t.quote}&rdquo;
                  </blockquote>
                  <div>
                    <p className="text-sm font-semibold">{t.author}</p>
                    <p className="text-xs text-muted-foreground">{t.role}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </section>

        {/* Pricing */}
        <section id="pricing" className="bg-muted/30 py-20 sm:py-28">
          <div className="container">
            <div className="mx-auto max-w-2xl text-center mb-16">
              <h2 className="text-3xl font-bold tracking-tight sm:text-4xl">
                Simple, Transparent Pricing
              </h2>
              <p className="mt-4 text-muted-foreground">
                Choose the plan that fits your team. Upgrade anytime as you grow.
              </p>
            </div>

            {tiersLoading ? (
              <div className="grid gap-8 lg:grid-cols-3">
                {[1, 2, 3].map((i) => (
                  <div key={i} className="rounded-2xl border bg-card p-8 animate-pulse">
                    <div className="h-6 w-24 bg-muted rounded mb-4" />
                    <div className="h-10 w-32 bg-muted rounded mb-2" />
                    <div className="h-4 w-48 bg-muted rounded mb-6" />
                    <div className="space-y-3 mb-8">
                      {[...Array(5)].map((_, j) => (
                        <div key={j} className="h-4 bg-muted rounded" style={{ width: `${70 + Math.random() * 30}%` }} />
                      ))}
                    </div>
                    <div className="h-12 w-full bg-muted rounded-lg" />
                  </div>
                ))}
              </div>
            ) : (
            <div className="grid gap-8 lg:grid-cols-3">
              {tiers.map((tier) => {
                const priceParts = tier.priceDisplay.match(/^(\$?\d+|[A-Za-z]+)(.*)$/)
                const priceMain = priceParts ? priceParts[1] : tier.priceDisplay
                const pricePeriod = priceParts && priceParts[2] ? priceParts[2] : ""
                const isEnterprise = tier.displayName.toLowerCase() === "enterprise"

                return (
                <div
                  key={tier.id}
                  className={`relative rounded-2xl border p-8 ${
                    tier.highlighted
                      ? "border-primary bg-card shadow-lg shadow-primary/10 ring-1 ring-primary"
                      : "bg-card"
                  }`}
                >
                  {tier.highlighted && (
                    <div className="absolute -top-3 left-1/2 -translate-x-1/2">
                      <span className="inline-flex items-center rounded-full bg-primary px-3 py-1 text-xs font-semibold text-primary-foreground">
                        Most Popular
                      </span>
                    </div>
                  )}
                  <h3 className="text-lg font-semibold">{tier.displayName}</h3>
                  <div className="mt-4 flex items-baseline gap-1">
                    <span className="text-4xl font-extrabold">{priceMain}</span>
                    <span className="text-sm text-muted-foreground">{pricePeriod}</span>
                  </div>
                  <p className="mt-2 text-sm text-muted-foreground">{tier.description}</p>
                  <ul className="mt-6 space-y-3">
                    {tier.features.map((f) => (
                      <li key={f} className="flex items-start gap-2 text-sm">
                        <CheckCircle2 className="mt-0.5 h-4 w-4 flex-shrink-0 text-primary" />
                        {f}
                      </li>
                    ))}
                  </ul>
                  <Button
                    className="mt-8 w-full"
                    variant={tier.highlighted ? "default" : "outline"}
                    size="lg"
                    asChild
                  >
                    <a href={isEnterprise ? "mailto:sales@onboardflow.com" : "/get-started"}>
                      {tier.ctaText}
                    </a>
                  </Button>
                </div>
                )
              })}
            </div>
            )}
          </div>
        </section>

        {/* FAQ */}
        <section id="faq" className="py-20 sm:py-28">
          <div className="container">
            <div className="mx-auto max-w-2xl text-center mb-16">
              <h2 className="text-3xl font-bold tracking-tight sm:text-4xl">
                Frequently Asked Questions
              </h2>
            </div>
            <div className="mx-auto max-w-3xl divide-y">
              {faqs.map((faq) => (
                <details key={faq.q} className="group py-5">
                  <summary className="flex cursor-pointer items-center justify-between text-base font-semibold group-open:text-primary">
                    {faq.q}
                    <span className="ml-4 text-xl font-light transition-transform group-open:rotate-45">
                      +
                    </span>
                  </summary>
                  <p className="mt-3 text-sm text-muted-foreground leading-relaxed">{faq.a}</p>
                </details>
              ))}
            </div>
          </div>
        </section>

        {/* CTA */}
        <section className="bg-primary py-20 sm:py-28">
          <div className="container text-center">
            <h2 className="text-3xl font-bold tracking-tight text-primary-foreground sm:text-4xl">
              Ready to Transform Your Onboarding?
            </h2>
            <p className="mx-auto mt-4 max-w-xl text-primary-foreground/80">
              Join 500+ companies that have automated their employee onboarding with OnboardFlow.
              Start your free trial today.
            </p>
            <div className="mt-8 flex flex-col items-center gap-4 sm:flex-row sm:justify-center">
              <Button size="xl" variant="secondary" asChild>
                <a href="/get-started">
                  Get Started Free
                  <ArrowRight className="ml-1 h-4 w-4" />
                </a>
              </Button>
              <Button
                size="xl"
                variant="outline"
                className="border-primary-foreground/20 text-primary-foreground hover:bg-primary-foreground/10"
                asChild
              >
                <a href="mailto:sales@onboardflow.com">Talk to Sales</a>
              </Button>
            </div>
          </div>
        </section>
      </main>

      {/* Footer */}
      <footer className="border-t bg-muted/30 py-12">
        <div className="container">
          <div className="flex flex-col items-center justify-between gap-6 md:flex-row">
            <div className="flex items-center gap-2">
              <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary">
                <CheckCircle2 className="h-4 w-4 text-primary-foreground" />
              </div>
              <span className="text-lg font-bold">OnboardFlow</span>
            </div>
            <div className="flex gap-8 text-sm text-muted-foreground">
              <a href="#" className="hover:text-foreground transition-colors">
                Privacy Policy
              </a>
              <a href="#" className="hover:text-foreground transition-colors">
                Terms of Service
              </a>
              <a href="#" className="hover:text-foreground transition-colors">
                Security
              </a>
              <a href="mailto:support@onboardflow.com" className="hover:text-foreground transition-colors">
                Contact
              </a>
            </div>
            <p className="text-xs text-muted-foreground">
              &copy; {new Date().getFullYear()} OnboardFlow. All rights reserved.
            </p>
          </div>
        </div>
      </footer>
    </div>
  )
}

export default LandingPage
