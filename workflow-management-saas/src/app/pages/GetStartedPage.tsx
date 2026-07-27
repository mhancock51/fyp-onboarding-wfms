import { useEffect, useState } from "react"
import { useNavigate } from "react-router-dom"
import {
  ArrowLeft,
  ArrowRight,
  Building2,
  CheckCircle2,
  CreditCard,
  Loader2,
  User,
  Shield,
} from "lucide-react"
import { Button } from "@/components/ui/button"

const BACKEND_API_URL = import.meta.env.VITE_BACKEND_API_URL || "http://localhost:5000/api"

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

// Static fallback tiers used only when the API is unreachable.
// In normal operation, pricing is fetched dynamically from the backend →
// Stripe so prices and features are always up to date.
const fallbackTiers: PricingTier[] = [
  {
    id: "tier-1-subscription",
    name: "tier-1-subscription",
    displayName: "Starter",
    description: "For small teams getting started with structured onboarding.",
    unitAmount: 4900,
    currency: "USD",
    interval: "month",
    priceDisplay: "$49/month",
    highlighted: false,
    ctaText: "Start Free Trial",
    features: [
      "Up to 5 workflow templates",
      "10 active onboardings",
      "Basic task templates",
      "Email notifications",
      "100 MB document storage",
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

const STEPS = [
  { icon: User, label: "Account" },
  { icon: CreditCard, label: "Plan" },
  { icon: Building2, label: "Organisation" },
]

export default function GetStartedPage() {
  const navigate = useNavigate()

  // Step state
  const [step, setStep] = useState(0)

  // Form state
  const [displayName, setDisplayName] = useState("")
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")
  const [organisationName, setOrganisationName] = useState("")
  const [selectedTierId, setSelectedTierId] = useState("tier-2-subscription")

  // UI state
  const [loading, setLoading] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [tiers, setTiers] = useState<PricingTier[]>(fallbackTiers)
  const [tiersLoading, setTiersLoading] = useState(true)
  const [checkoutUrl, setCheckoutUrl] = useState<string | null>(null)

  // Fetch pricing tiers on mount
  useEffect(() => {
    let cancelled = false
    async function fetchPricing() {
      try {
        const res = await fetch(`${BACKEND_API_URL}/pricing/tiers`)
        if (!res.ok) throw new Error(`HTTP ${res.status}`)
        const json = await res.json()
        if (!cancelled && json.success && Array.isArray(json.data) && json.data.length > 0) {
          setTiers(json.data as PricingTier[])
        }
      } catch (err) {
        console.warn("Failed to fetch pricing from API, using fallback tiers:", err)
      } finally {
        if (!cancelled) setTiersLoading(false)
      }
    }
    fetchPricing()
    return () => { cancelled = true }
  }, [])

  const selectedTier = tiers.find((t) => t.id === selectedTierId) || tiers[1]

  function canProceedFromStep1(): boolean {
    return (
      displayName.trim().length > 0 &&
      email.trim().length > 0 &&
      password.length >= 6 &&
      password === confirmPassword
    )
  }

  function canProceedFromStep3(): boolean {
    return organisationName.trim().length > 0
  }

  function handleNext() {
    setError(null)
    if (step === 0 && !canProceedFromStep1()) {
      if (password !== confirmPassword) setError("Passwords do not match.")
      else if (password.length < 6) setError("Password must be at least 6 characters.")
      else setError("Please fill in all account fields.")
      return
    }
    if (step === 1 && !selectedTierId) {
      setError("Please select a plan.")
      return
    }
    if (step === 2 && !canProceedFromStep3()) {
      setError("Please enter an organisation name.")
      return
    }
    setStep((s) => s + 1)
  }

  function handleBack() {
    setError(null)
    setStep((s) => Math.max(0, s - 1))
  }

  async function handleSubmit() {
    if (!canProceedFromStep3()) {
      setError("Please enter an organisation name.")
      return
    }
    setSubmitting(true)
    setError(null)

    try {
      const res = await fetch(`${BACKEND_API_URL}/getstarted`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          displayName: displayName.trim(),
          email: email.trim(),
          password,
          organisationName: organisationName.trim(),
          subscriptionTierId: selectedTierId,
        }),
      })

      const json = await res.json()

      if (!res.ok || !json.success) {
        throw new Error(json.error || json.message || "Sign-up failed. Please try again.")
      }

      // Store JWT for post-checkout login
      if (json.data?.jwtToken) {
        localStorage.setItem("jwtToken", json.data.jwtToken)
        localStorage.setItem("user", JSON.stringify({
          displayName: json.data.displayName,
          email: json.data.email,
          tenantId: json.data.tenantId,
        }))
      }

      setCheckoutUrl(json.data?.checkoutUrl || null)
      setStep(3) // move to confirmation step
    } catch (err: any) {
      setError(err.message || "An unexpected error occurred.")
    } finally {
      setSubmitting(false)
    }
  }

  function handleGoToCheckout() {
    if (checkoutUrl) {
      window.location.href = checkoutUrl
    }
  }

  // --- Step indicators ---
  function renderSteps() {
    return (
      <div className="flex items-center justify-center gap-2 mb-10">
        {STEPS.map((s, i) => {
          const isActive = i === step && step < 3
          const isComplete = i < step || (i === step && step === 3)
          const Icon = s.icon
          return (
            <div key={s.label} className="flex items-center gap-2">
              <div
                className={`flex items-center gap-2 rounded-full px-3 py-1.5 text-sm font-medium transition-colors ${
                  isActive
                    ? "bg-primary text-primary-foreground"
                    : isComplete
                    ? "bg-primary/10 text-primary"
                    : "bg-muted text-muted-foreground"
                }`}
              >
                <Icon className="h-4 w-4" />
                <span className="hidden sm:inline">{s.label}</span>
              </div>
              {i < STEPS.length - 1 && (
                <div className={`h-0.5 w-8 rounded ${i < step ? "bg-primary" : "bg-muted"}`} />
              )}
            </div>
          )
        })}
      </div>
    )
  }

  // --- Step 0: Account Details ---
  function renderAccountStep() {
    return (
      <div className="space-y-5">
        <div className="text-center">
          <h2 className="text-2xl font-bold tracking-tight">Create your account</h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Set up your admin account to get started with OnboardFlow.
          </p>
        </div>

        <div className="space-y-4">
          <div>
            <label htmlFor="displayName" className="block text-sm font-medium mb-1.5">
              Full Name
            </label>
            <input
              id="displayName"
              type="text"
              value={displayName}
              onChange={(e) => setDisplayName(e.target.value)}
              placeholder="Jane Smith"
              className="w-full rounded-lg border border-input bg-background px-3 py-2.5 text-sm shadow-sm transition-colors placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary"
            />
          </div>

          <div>
            <label htmlFor="email" className="block text-sm font-medium mb-1.5">
              Work Email
            </label>
            <input
              id="email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="jane@company.com"
              className="w-full rounded-lg border border-input bg-background px-3 py-2.5 text-sm shadow-sm transition-colors placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary"
            />
          </div>

          <div>
            <label htmlFor="password" className="block text-sm font-medium mb-1.5">
              Password
            </label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="At least 6 characters"
              className="w-full rounded-lg border border-input bg-background px-3 py-2.5 text-sm shadow-sm transition-colors placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary"
            />
          </div>

          <div>
            <label htmlFor="confirmPassword" className="block text-sm font-medium mb-1.5">
              Confirm Password
            </label>
            <input
              id="confirmPassword"
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              placeholder="Re-enter your password"
              className="w-full rounded-lg border border-input bg-background px-3 py-2.5 text-sm shadow-sm transition-colors placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary"
            />
          </div>
        </div>
      </div>
    )
  }

  // --- Step 1: Select Plan ---
  function renderPlanStep() {
    if (tiersLoading) {
      return (
        <div className="flex items-center justify-center py-16">
          <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
        </div>
      )
    }

    return (
      <div className="space-y-6">
        <div className="text-center">
          <h2 className="text-2xl font-bold tracking-tight">Choose your plan</h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Select the plan that fits your team. You can change or cancel anytime.
          </p>
        </div>

        <div className="grid gap-5 lg:grid-cols-3">
          {tiers.map((tier) => {
            const isEnterprise = tier.displayName.toLowerCase() === "enterprise"
            const priceParts = tier.priceDisplay.match(/^(\$?\d+|[A-Za-z]+)(.*)$/)
            const priceMain = priceParts ? priceParts[1] : tier.priceDisplay
            const pricePeriod = priceParts && priceParts[2] ? priceParts[2] : ""

            return (
              <button
                key={tier.id}
                type="button"
                disabled={isEnterprise}
                onClick={() => setSelectedTierId(tier.id)}
                className={`relative rounded-2xl border p-6 text-left transition-all ${
                  selectedTierId === tier.id
                    ? "border-primary bg-primary/5 shadow-lg shadow-primary/10 ring-1 ring-primary"
                    : "border-border bg-card hover:border-primary/40 hover:shadow-md"
                } ${isEnterprise ? "cursor-not-allowed opacity-70" : "cursor-pointer"}`}
              >
                {tier.highlighted && (
                  <div className="absolute -top-3 left-1/2 -translate-x-1/2">
                    <span className="inline-flex items-center rounded-full bg-primary px-3 py-1 text-xs font-semibold text-primary-foreground">
                      Most Popular
                    </span>
                  </div>
                )}

                <h3 className="text-lg font-semibold">{tier.displayName}</h3>
                <div className="mt-3 flex items-baseline gap-1">
                  <span className="text-3xl font-extrabold">{priceMain}</span>
                  <span className="text-sm text-muted-foreground">{pricePeriod}</span>
                </div>
                <p className="mt-1.5 text-sm text-muted-foreground">{tier.description}</p>

                <ul className="mt-4 space-y-2">
                  {tier.features.map((f) => (
                    <li key={f} className="flex items-start gap-2 text-sm">
                      <CheckCircle2 className="mt-0.5 h-3.5 w-3.5 flex-shrink-0 text-primary" />
                      {f}
                    </li>
                  ))}
                </ul>

                {selectedTierId === tier.id && (
                  <div className="mt-4 rounded-lg bg-primary/10 px-3 py-1.5 text-center text-xs font-medium text-primary">
                    Selected
                  </div>
                )}
              </button>
            )
          })}
        </div>
      </div>
    )
  }

  // --- Step 2: Organisation ---
  function renderOrganisationStep() {
    return (
      <div className="space-y-5">
        <div className="text-center">
          <h2 className="text-2xl font-bold tracking-tight">Name your organisation</h2>
          <p className="mt-2 text-sm text-muted-foreground">
            This is the name that will appear across your OnboardFlow workspace.
          </p>
        </div>

        <div>
          <label htmlFor="orgName" className="block text-sm font-medium mb-1.5">
            Organisation Name
          </label>
          <input
            id="orgName"
            type="text"
            value={organisationName}
            onChange={(e) => setOrganisationName(e.target.value)}
            placeholder="Acme Corp"
            className="w-full rounded-lg border border-input bg-background px-3 py-2.5 text-sm shadow-sm transition-colors placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary"
          />
        </div>

        {/* Summary card */}
        <div className="rounded-xl border bg-muted/30 p-5 space-y-3">
          <h3 className="text-sm font-semibold text-foreground">Summary</h3>
          <div className="space-y-2 text-sm">
            <div className="flex justify-between">
              <span className="text-muted-foreground">Account</span>
              <span className="font-medium">{displayName} ({email})</span>
            </div>
            <div className="flex justify-between">
              <span className="text-muted-foreground">Plan</span>
              <span className="font-medium">
                {selectedTier.displayName} — {selectedTier.priceDisplay}
              </span>
            </div>
            <div className="flex justify-between">
              <span className="text-muted-foreground">Organisation</span>
              <span className="font-medium">{organisationName || "—"}</span>
            </div>
          </div>
        </div>

        <Button
          onClick={() => { void handleSubmit() }}
          disabled={submitting || !canProceedFromStep3()}
          className="w-full"
          size="lg"
        >
          {submitting ? (
            <>
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Setting up...
            </>
          ) : (
            <>
              <Shield className="mr-2 h-4 w-4" />
              Create Account & Continue to Payment
            </>
          )}
        </Button>
      </div>
    )
  }

  // --- Step 3: Confirmation (checkout) ---
  function renderConfirmationStep() {
    return (
      <div className="space-y-6 text-center">
        <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-primary/10">
          <CheckCircle2 className="h-8 w-8 text-primary" />
        </div>

        <div>
          <h2 className="text-2xl font-bold tracking-tight">Account created!</h2>
          <p className="mt-2 text-sm text-muted-foreground">
            Your workspace is ready. Complete your subscription to activate all features.
          </p>
        </div>

        <div className="rounded-xl border bg-muted/30 p-5 text-left space-y-2 text-sm">
          <div className="flex justify-between">
            <span className="text-muted-foreground">Account</span>
            <span className="font-medium">{displayName}</span>
          </div>
          <div className="flex justify-between">
            <span className="text-muted-foreground">Plan</span>
            <span className="font-medium">{selectedTier.displayName}</span>
          </div>
          <div className="flex justify-between">
            <span className="text-muted-foreground">Organisation</span>
            <span className="font-medium">{organisationName}</span>
          </div>
        </div>

        <div className="flex flex-col gap-3">
          <Button onClick={handleGoToCheckout} size="lg" className="w-full">
            <CreditCard className="mr-2 h-4 w-4" />
            Go to Stripe Checkout
          </Button>
          <p className="text-xs text-muted-foreground">
            You&apos;ll be redirected to Stripe&apos;s secure checkout to complete your subscription.
            After payment, you can sign in with your email and password.
          </p>
        </div>
      </div>
    )
  }

  // --- Main render ---
  return (
    <div className="flex min-h-screen flex-col bg-white">
      {/* Header */}
      <header className="sticky top-0 z-50 border-b bg-white/95 backdrop-blur">
        <div className="container flex h-16 items-center justify-between">
          <button
            onClick={() => navigate("/")}
            className="flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors"
          >
            <ArrowLeft className="h-4 w-4" />
            Back to Home
          </button>
          <div className="flex items-center gap-2">
            <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary">
              <CheckCircle2 className="h-4 w-4 text-primary-foreground" />
            </div>
            <span className="text-lg font-bold">OnboardFlow</span>
          </div>
          <div className="w-24" /> {/* spacer */}
        </div>
      </header>

      <main className="flex-1 flex items-center justify-center py-12 px-4">
        <div className="w-full max-w-5xl">
          {step < 3 && renderSteps()}

          <div className="rounded-2xl border bg-card p-8 shadow-sm">
            {step === 0 && renderAccountStep()}
            {step === 1 && renderPlanStep()}
            {step === 2 && renderOrganisationStep()}
            {step === 3 && renderConfirmationStep()}

            {error && (
              <div className="mt-4 rounded-lg bg-destructive/10 px-4 py-3 text-sm text-destructive">
                {error}
              </div>
            )}

            {/* Navigation buttons for steps 0-2 */}
            {step < 3 && (
              <div className="mt-8 flex items-center justify-between">
                {step > 0 ? (
                  <Button variant="ghost" onClick={handleBack} disabled={submitting}>
                    <ArrowLeft className="mr-2 h-4 w-4" />
                    Back
                  </Button>
                ) : (
                  <div />
                )}
                {step < 2 && (
                  <Button onClick={handleNext}>
                    Continue
                    <ArrowRight className="ml-2 h-4 w-4" />
                  </Button>
                )}
              </div>
            )}
          </div>

          {step < 3 && (
            <p className="mt-6 text-center text-xs text-muted-foreground">
              By continuing, you agree to OnboardFlow&apos;s{" "}
              <a href="#" className="underline underline-offset-2 hover:text-foreground">
                Terms of Service
              </a>{" "}
              and{" "}
              <a href="#" className="underline underline-offset-2 hover:text-foreground">
                Privacy Policy
              </a>
              .
            </p>
          )}
        </div>
      </main>
    </div>
  )
}
